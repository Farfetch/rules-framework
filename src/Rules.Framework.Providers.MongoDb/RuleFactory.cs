namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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
                .WithCondition(_ => ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null)
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

        private static IConditionNode<TConditionType> ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeDataModel as ValueConditionNodeDataModel);
            }

            var composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;
            var childConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes;
            var count = childConditionNodeDataModels.Length;
            var childConditionNodes = new IConditionNode<TConditionType>[count];
            for (int i = 0; i < count; i++)
            {
                childConditionNodes[i] = ConvertConditionNode(childConditionNodeDataModels[i]);
            }

            var composedConditionNode = new ComposedConditionNode<TConditionType>(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes);
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

        private static ValueConditionNode<TConditionType> CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            var operand = conditionNodeDataModel.DataType switch
            {
                DataTypes.Integer => Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.Decimal => Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.String => Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.Boolean => Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.ArrayInteger or DataTypes.ArrayDecimal or DataTypes.ArrayString or DataTypes.ArrayBoolean => conditionNodeDataModel.Operand,
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };

            var valueConditionNode = new ValueConditionNode<TConditionType>(conditionNodeDataModel.DataType, conditionType, conditionNodeDataModel.Operator, operand);

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