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
                .WithActive(ruleDataModel.Active ?? true)
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

            var content = rule.ContentContainer.GetContentAs<object>();
            var serializedContent = this.contentSerializationProvider.GetContentSerializer(rule.ContentContainer.ContentType).Serialize(content);

            var ruleDataModel = new RuleDataModel
            {
                Content = serializedContent,
                ContentType = Convert.ToString(rule.ContentContainer.ContentType, CultureInfo.InvariantCulture),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                Active = rule.Active,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null,
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> ConvertConditionNode(
            IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel);
            }

            var composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

            var composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                .WithLogicalOperator(composedConditionNodeDataModel.LogicalOperator);
            var childConditionNodes = composedConditionNodeDataModel.ChildConditionNodes;
            var count = childConditionNodes.Length;
            var i = -1;
            while (++i < count)
            {
                composedConditionNodeBuilder.AddCondition(cnb => ConvertConditionNode(cnb, childConditionNodes[i]));
            }

            var composedConditionNode = composedConditionNodeBuilder.Build();
            foreach (var property in composedConditionNodeDataModel.Properties)
            {
                composedConditionNode.Properties[property.Key] = property.Value;
            }

            return composedConditionNode;
        }

        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode<TConditionType> valueConditionNode)
        {
            var properties = FilterProperties(valueConditionNode.Properties);

            return new ValueConditionNodeDataModel
            {
                ConditionType = Convert.ToString(valueConditionNode.ConditionType, CultureInfo.InvariantCulture),
                LogicalOperator = LogicalOperators.Eval,
                DataType = valueConditionNode.DataType,
                Operand = valueConditionNode.Operand,
                Operator = valueConditionNode.Operator,
                Properties = properties,
            };
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ValueConditionNodeDataModel conditionNodeDataModel)
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            var valueConditionNode = conditionNodeDataModel.DataType switch
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

            foreach (var property in conditionNodeDataModel.Properties)
            {
                valueConditionNode.Properties[property.Key] = property.Value;
            }

            return valueConditionNode;
        }

        private static IDictionary<string, object> FilterProperties(IDictionary<string, object> source)
        {
            var properties = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var property in source)
            {
                if (property.Key.StartsWith("_compilation", StringComparison.Ordinal))
                {
                    continue;
                }

                properties[property.Key] = property.Value;
            }

            return properties;
        }

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private ConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode<TConditionType> composedConditionNode)
        {
            var conditionNodeDataModels = new ConditionNodeDataModel[composedConditionNode.ChildConditionNodes.Count()];
            var i = 0;
            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels[i++] = this.ConvertConditionNode(child);
            }

            var properties = FilterProperties(composedConditionNode.Properties);

            return new ComposedConditionNodeDataModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
                Properties = properties,
            };
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode<TConditionType> conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode(conditionNode as ValueConditionNode<TConditionType>);
            }

            return ConvertComposedConditionNode(conditionNode as ComposedConditionNode<TConditionType>);
        }
    }
}
