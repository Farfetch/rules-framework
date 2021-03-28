namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using MongoDB.Bson;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Serialization;

    internal class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        private readonly IContentSerializationProvider<TContentType> contentSerializationProvider;

        public RuleFactory(IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            this.contentSerializationProvider = contentSerializationProvider;
        }

        public Rule<TContentType, TConditionType> CreateRule(RuleDataModel ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            TContentType contentType = Parse<TContentType>(ruleDataModel.ContentType);

            RuleBuilderResult<TContentType, TConditionType> ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithCondition(cnb => ruleDataModel.RootCondition is { } ? this.ConvertConditionNode(cnb, ruleDataModel.RootCondition) : null)
                .WithSerializedContent(contentType, (object)ruleDataModel.Content, this.contentSerializationProvider)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule ID: {ruleDataModel.Id.ToString()}", ruleBuilderResult.Errors);
            }

            ruleBuilderResult.Rule.Priority = ruleDataModel.Priority;

            if (ruleBuilderResult.Rule.Priority <= 0)
            {
                throw new InvalidRuleException(
                    $"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}",
                    new[] { $"Loaded rule priority number is invalid: {ruleBuilderResult.Rule.Priority}." });
            }

            return ruleBuilderResult.Rule;
        }

        public RuleDataModel CreateRule(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            dynamic content = rule.ContentContainer.GetContentAs<dynamic>();

            RuleDataModel ruleDataModel = new RuleDataModel
            {
                Content = content,
                ContentType = Convert.ToString(rule.ContentContainer.ContentType, CultureInfo.InvariantCulture),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Id = ObjectId.GenerateNewId(),
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? this.ConvertConditionNode(rule.RootCondition) : null
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ValueConditionNodeDataModel conditionNodeDataModel)
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            return conditionNodeDataModel.DataType switch
            {
                DataTypes.Integer => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<int>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),
                DataTypes.Decimal => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<decimal>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.String => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<string>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.Boolean => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<bool>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };
        }

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode<TConditionType> ConvertConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel);
            }
            else
            {
                ComposedConditionNodeDataModel composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

                IComposedConditionNodeBuilder<TConditionType> composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                    .WithLogicalOperator(composedConditionNodeDataModel.LogicalOperator);

                foreach (ConditionNodeDataModel child in composedConditionNodeDataModel.ChildConditionNodes)
                {
                    composedConditionNodeBuilder.AddCondition(cnb => this.ConvertConditionNode(cnb, child));
                }

                return composedConditionNodeBuilder.Build();
            }
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode<TConditionType> conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return conditionNode switch
                {
                    BooleanConditionNode<TConditionType> booleanConditionNode => new ValueConditionNodeDataModel
                    {
                        ConditionType = Convert.ToString(booleanConditionNode.ConditionType, CultureInfo.InvariantCulture),
                        LogicalOperator = LogicalOperators.Eval,
                        DataType = booleanConditionNode.DataType,
                        Operand = booleanConditionNode.Operand,
                        Operator = booleanConditionNode.Operator
                    },
                    DecimalConditionNode<TConditionType> decimalConditionNode => new ValueConditionNodeDataModel
                    {
                        ConditionType = Convert.ToString(decimalConditionNode.ConditionType, CultureInfo.InvariantCulture),
                        LogicalOperator = LogicalOperators.Eval,
                        DataType = decimalConditionNode.DataType,
                        Operand = decimalConditionNode.Operand,
                        Operator = decimalConditionNode.Operator
                    },
                    IntegerConditionNode<TConditionType> integerConditionNode => new ValueConditionNodeDataModel
                    {
                        ConditionType = Convert.ToString(integerConditionNode.ConditionType, CultureInfo.InvariantCulture),
                        LogicalOperator = LogicalOperators.Eval,
                        DataType = integerConditionNode.DataType,
                        Operand = integerConditionNode.Operand,
                        Operator = integerConditionNode.Operator
                    },
                    StringConditionNode<TConditionType> stringConditionNode => new ValueConditionNodeDataModel
                    {
                        ConditionType = Convert.ToString(stringConditionNode.ConditionType, CultureInfo.InvariantCulture),
                        LogicalOperator = LogicalOperators.Eval,
                        DataType = stringConditionNode.DataType,
                        Operand = stringConditionNode.Operand,
                        Operator = stringConditionNode.Operator
                    },
                    _ => throw new NotSupportedException($"Unsupported value condition node type: {conditionNode.GetType().FullName}."),
                };
            }
            else
            {
                ComposedConditionNode<TConditionType> composedConditionNode = conditionNode as ComposedConditionNode<TConditionType>;

                List<ConditionNodeDataModel> conditionNodeDataModels = new List<ConditionNodeDataModel>(composedConditionNode.ChildConditionNodes.Count());
                foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
                {
                    conditionNodeDataModels.Add(this.ConvertConditionNode(child));
                }

                return new ComposedConditionNodeDataModel
                {
                    ChildConditionNodes = conditionNodeDataModels,
                    LogicalOperator = composedConditionNode.LogicalOperator
                };
            }
        }
    }
}