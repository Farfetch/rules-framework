namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Serialization;

    internal sealed class RuleFactory : IRuleFactory
    {
        private readonly IContentSerializationProvider contentSerializationProvider;

        public RuleFactory(IContentSerializationProvider contentSerializationProvider)
        {
            this.contentSerializationProvider = contentSerializationProvider;
        }

        public Rule CreateRule(RuleDataModel ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            var ruleBuilderResult = Rule.Create(ruleDataModel.Name)
                .OnRuleset(ruleDataModel.Ruleset)
                .SetContent((object)ruleDataModel.Content, this.contentSerializationProvider)
                .Since(ruleDataModel.DateBegin)
                .Until(ruleDataModel.DateEnd)
                .WithActive(ruleDataModel.Active ?? true)
                .ApplyWhen(_ => ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null)
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

        public RuleDataModel CreateRule(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var content = rule.ContentContainer.GetContentAs<object>();
            var serializedContent = this.contentSerializationProvider.GetContentSerializer(rule.Ruleset).Serialize(content);

            var ruleDataModel = new RuleDataModel
            {
                Active = rule.Active,
                Content = serializedContent,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null,
                Ruleset = rule.Ruleset,
            };

            return ruleDataModel;
        }

        private static IConditionNode ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeDataModel as ValueConditionNodeDataModel);
            }

            var composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;
            var childConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes;
            var count = childConditionNodeDataModels.Length;
            var childConditionNodes = new IConditionNode[count];
            for (var i = 0; i < count; i++)
            {
                childConditionNodes[i] = ConvertConditionNode(childConditionNodeDataModels[i]);
            }

            var composedConditionNode = new ComposedConditionNode(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes);
            foreach (var property in composedConditionNodeDataModel.Properties)
            {
                composedConditionNode.Properties[property.Key] = property.Value;
            }

            return composedConditionNode;
        }

        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode valueConditionNode)
        {
            var properties = FilterProperties(valueConditionNode.Properties);

            return new ValueConditionNodeDataModel
            {
                Condition = Convert.ToString(valueConditionNode.Condition, CultureInfo.InvariantCulture),
                LogicalOperator = LogicalOperators.Eval,
                DataType = valueConditionNode.DataType,
                Operand = valueConditionNode.Operand,
                Operator = valueConditionNode.Operator,
                Properties = properties,
            };
        }

        private static ValueConditionNode CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            var operand = conditionNodeDataModel.DataType switch
            {
                DataTypes.Integer => Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.Decimal => Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.String => Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.Boolean => Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture),
                DataTypes.ArrayInteger or DataTypes.ArrayDecimal or DataTypes.ArrayString or DataTypes.ArrayBoolean => conditionNodeDataModel.Operand,
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };

            var valueConditionNode = new ValueConditionNode(
                conditionNodeDataModel.DataType,
                conditionNodeDataModel.Condition,
                conditionNodeDataModel.Operator,
                operand);

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

        private ComposedConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode composedConditionNode)
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

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode(conditionNode as ValueConditionNode);
            }

            return ConvertComposedConditionNode(conditionNode as ComposedConditionNode);
        }
    }
}