namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class RuleFactory : IRuleFactory
    {
        public Rule CreateRule(RuleDataModel ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            var contentContainer = new ContentContainer((_) => ruleDataModel.Content);

            var rule = new Rule
            {
                Active = ruleDataModel.Active,
                ContentContainer = contentContainer,
                ContentType = ruleDataModel.ContentType,
                DateBegin = ruleDataModel.DateBegin,
                DateEnd = ruleDataModel.DateEnd,
                Name = ruleDataModel.Name,
                Priority = ruleDataModel.Priority,
                RootCondition = ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null!,
            };

            return rule;
        }

        public RuleDataModel CreateRule(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var content = rule.ContentContainer.GetContentAs<dynamic>();

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                ContentType = rule.ContentType,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                Active = rule.Active,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null!,
            };

            return ruleDataModel;
        }

        private static IConditionNode ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode((ValueConditionNodeDataModel)conditionNodeDataModel);
            }

            var composedConditionNodeDataModel = (ComposedConditionNodeDataModel)conditionNodeDataModel;
            var count = composedConditionNodeDataModel.ChildConditionNodes.Length;
            var childConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes;
            var childConditionNodes = new IConditionNode[count];
            var i = -1;

            while (++i < count)
            {
                childConditionNodes[i] = ConvertConditionNode(childConditionNodeDataModels[i]);
            }

            return new ComposedConditionNode(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode valueConditionNode)
        {
            return new ValueConditionNodeDataModel
            {
                ConditionType = valueConditionNode.ConditionType,
                LogicalOperator = LogicalOperators.Eval,
                DataType = valueConditionNode.DataType,
                Operand = valueConditionNode.Operand,
                Operator = valueConditionNode.Operator,
                Properties = new PropertiesDictionary(valueConditionNode.Properties),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IConditionNode CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            return new ValueConditionNode(
                conditionNodeDataModel.DataType,
                conditionNodeDataModel.ConditionType,
                conditionNodeDataModel.Operator,
                conditionNodeDataModel.Operand,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode composedConditionNode)
        {
            var conditionNodeDataModels = new ConditionNodeDataModel[composedConditionNode.ChildConditionNodes.Count()];
            var i = 0;

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels[i++] = ConvertConditionNode(child);
            }

            return new ComposedConditionNodeDataModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
                Properties = new PropertiesDictionary(composedConditionNode.Properties),
            };
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode((ValueConditionNode)conditionNode);
            }

            return ConvertComposedConditionNode((ComposedConditionNode)conditionNode);
        }
    }
}