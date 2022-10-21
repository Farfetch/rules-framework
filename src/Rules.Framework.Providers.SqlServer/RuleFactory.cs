namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Globalization;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Serialization;
    using Rules.Framework.SqlServer.Models;

    using Core = Rules.Framework.Core;
    using Model = Rules.Framework.SqlServer.Models;

    public class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        private readonly IContentSerializationProvider<TContentType> contentSerializationProvider;

        public RuleFactory(IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            this.contentSerializationProvider = contentSerializationProvider;
        }

        public Core.Rule<TContentType, TConditionType> CreateRule(Model.Rule ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            TContentType contentType = Parse<TContentType>(ruleDataModel.ContentTypeCode.ToString());

            RuleBuilderResult<TContentType, TConditionType> ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithCondition(cnb => ruleDataModel.ConditionNode is { } ? this.ConvertConditionNode(cnb, ruleDataModel.ConditionNode) : null) //TODO: replace ruleDataModel.ConditionNode by ruleDataModel.RootCondition
                .WithSerializedContent(contentType, (object)ruleDataModel.Content, this.contentSerializationProvider)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}", ruleBuilderResult.Errors);
            }

            ruleBuilderResult.Rule.Priority = ruleDataModel.Priority;
            ruleBuilderResult.Rule.ConditionNodeId = ruleDataModel.ConditionNodeId;

            if (ruleBuilderResult.Rule.Priority <= 0)
            {
                throw new InvalidRuleException(
                    $"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}",
                    new[] { $"Loaded rule priority number is invalid: {ruleBuilderResult.Rule.Priority}." });
            }

            return ruleBuilderResult.Rule;
        }

        public Model.Rule CreateRule(Core.Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            dynamic content = rule.ContentContainer.GetContentAs<dynamic>();

            Model.Rule ruleDataModel = new Model.Rule
            {
                Content = content,
                ContentTypeCode = Convert.ToInt32(rule.ContentContainer.ContentType, CultureInfo.InvariantCulture),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                ConditionNodeId = rule.ConditionNodeId,
                ConditionNode = rule.RootCondition is { } ? this.ConvertConditionNode(rule.RootCondition) : null //TODO: check this
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNode conditionNodeDataModel) //TODO replace conditionNode by ValueConditionNodeDataModel
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionTypeCode.ToString());
            return conditionNodeDataModel.DataTypeCode switch
            {
                //TODO: replace numbers by Enum
                1 => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<int>()
                    .WithComparisonOperator((Operators)conditionNodeDataModel.OperatorCode)
                    .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .WithInternalId(conditionNodeDataModel.Id)
                    .Build(),
                2 => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<decimal>()
                   .WithComparisonOperator((Operators)conditionNodeDataModel.OperatorCode)
                   .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .WithInternalId(conditionNodeDataModel.Id)
                   .Build(),
                3 => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<string>()
                   .WithComparisonOperator((Operators)conditionNodeDataModel.OperatorCode)
                   .SetOperand(Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .WithInternalId(conditionNodeDataModel.Id)
                   .Build(),
                4 => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<bool>()
                   .WithComparisonOperator((Operators)conditionNodeDataModel.OperatorCode)
                   .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .WithInternalId(conditionNodeDataModel.Id)
                    .Build(),
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };
        }

        //TODO: move to a common place
        private static T Parse<T>(string value)
           => (T)Parse(value, typeof(T));

        //TODO: move to a common place
        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private ConditionNode ConvertConditionNode(IConditionNode<TConditionType> rootCondition)
        {
            ConditionNode conditionNode = new ConditionNode();
            conditionNode.LogicalOperatorCode = (int)rootCondition.LogicalOperator;

            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var ValueConditionNode = rootCondition as ValueConditionNode<TConditionType>;

                conditionNode.Id = (long)ValueConditionNode.InternalId;
                conditionNode.DataTypeCode = (int)ValueConditionNode.DataType;
                conditionNode.ConditionTypeCode = Convert.ToInt32(ValueConditionNode.ConditionType, CultureInfo.InvariantCulture);
                conditionNode.Operand = Convert.ToString(ValueConditionNode.Operand, CultureInfo.InvariantCulture);
                conditionNode.OperatorCode = (int)ValueConditionNode.Operator;
            }

            var composedConditionNode = rootCondition as ComposedConditionNode<TConditionType>;

            if (composedConditionNode is object)
            {
                foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                {
                    var child = ConvertConditionNode(childConditionNode);

                    conditionNode.ConditionNodeRelations_ChildId
                        .Add(new ConditionNodeRelation
                        {
                            Child = child,
                            Owner = conditionNode
                        });
                }
            }

            return conditionNode;
        }

        private IConditionNode<TConditionType> ConvertConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNode conditionNodeDataModel) //TODO: replace conditionNode by ConditionNodeDataModel
        {
            if (conditionNodeDataModel.LogicalOperatorCode == (int)LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel); //TODO: replace conditionNode by ValueConditionNodeDataModel
            }
            else
            {
                //ComposedConditionNodeDataModel composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

                IComposedConditionNodeBuilder<TConditionType> composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                    .WithLogicalOperator((LogicalOperators)conditionNodeDataModel.LogicalOperatorCode);

                foreach (ConditionNodeRelation conditionNodeRelation in conditionNodeDataModel.ConditionNodeRelations_OwnerId)
                {
                    composedConditionNodeBuilder.AddCondition(cnb => this.ConvertConditionNode(cnb, conditionNodeRelation.Child));
                }

                return composedConditionNodeBuilder.Build();
            }
        }
    }
}