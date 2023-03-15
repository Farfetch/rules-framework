namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Serialization;

    internal sealed class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
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

            var contentType = Parse<TContentType>(ruleDataModel.ContentType);

            var ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithCondition(cnb => ruleDataModel.RootCondition is { } ? ConvertConditionNode(cnb, ruleDataModel.RootCondition) : null)
                .WithSerializedContent(contentType, (object)ruleDataModel.Content, this.contentSerializationProvider)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}", ruleBuilderResult.Errors);
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

            var content = rule.ContentContainer.GetContentAs<dynamic>();

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                ContentType = Convert.ToString(rule.ContentContainer.ContentType, CultureInfo.InvariantCulture),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null,
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> ConvertConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel);
            }

            ComposedConditionNodeDataModel composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

            IComposedConditionNodeBuilder<TConditionType> composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                .WithLogicalOperator(composedConditionNodeDataModel.LogicalOperator);

            foreach (ConditionNodeDataModel child in composedConditionNodeDataModel.ChildConditionNodes)
            {
                composedConditionNodeBuilder.AddCondition(cnb => ConvertConditionNode(cnb, child));
            }

            return composedConditionNodeBuilder.Build();
        }

        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode<TConditionType> valueConditionNode) => new ValueConditionNodeDataModel
        {
            ConditionType = Convert.ToString(valueConditionNode.ConditionType, CultureInfo.InvariantCulture),
            LogicalOperator = LogicalOperators.Eval,
            DataType = valueConditionNode.DataType,
            Operand = valueConditionNode.Operand,
            Operator = valueConditionNode.Operator,
        };

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

                DataTypes.ArrayInteger => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<IEnumerable<int>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<int>)
                    .Build(),
                DataTypes.ArrayDecimal => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<IEnumerable<decimal>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<decimal>)
                    .Build(),
                DataTypes.ArrayString => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<IEnumerable<string>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<string>)
                    .Build(),
                DataTypes.ArrayBoolean => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<IEnumerable<bool>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<bool>)
                    .Build(),
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };
        }

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private ConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode<TConditionType> composedConditionNode)
        {
            List<ConditionNodeDataModel> conditionNodeDataModels = new List<ConditionNodeDataModel>(composedConditionNode.ChildConditionNodes.Count());
            foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(this.ConvertConditionNode(child));
            }

            return new ComposedConditionNodeDataModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
            };
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode<TConditionType> conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return conditionNode switch
                {
                    ValueConditionNode<TConditionType> valueConditionNode => ConvertValueConditionNode(valueConditionNode),
                    _ => throw new NotSupportedException($"Unsupported value condition node type: {conditionNode.GetType().FullName}."),
                };
            }

            ComposedConditionNode<TConditionType> composedConditionNode = conditionNode as ComposedConditionNode<TConditionType>;
            return ConvertComposedConditionNode(composedConditionNode);
        }
    }
}