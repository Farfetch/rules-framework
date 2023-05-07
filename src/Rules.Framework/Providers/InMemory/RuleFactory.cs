namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> CreateRule(RuleDataModel<TContentType, TConditionType> ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            var contentContainer = new ContentContainer<TContentType>(ruleDataModel.ContentType, (_) => ruleDataModel.Content);

            var rule = new Rule<TContentType, TConditionType>()
            {
                ContentContainer = contentContainer,
                DateBegin = ruleDataModel.DateBegin,
                DateEnd = ruleDataModel.DateEnd,
                Name = ruleDataModel.Name,
                Priority = ruleDataModel.Priority,
                RootCondition = ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null!,
            };

            return rule;
        }

        public RuleDataModel<TContentType, TConditionType> CreateRule(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var content = rule.ContentContainer.GetContentAs<dynamic>();

            var ruleDataModel = new RuleDataModel<TContentType, TConditionType>
            {
                Content = content,
                ContentType = rule.ContentContainer.ContentType,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null!,
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> ConvertConditionNode(ConditionNodeDataModel<TConditionType> conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode((ValueConditionNodeDataModel<TConditionType>)conditionNodeDataModel);
            }

            var composedConditionNodeDataModel = (ComposedConditionNodeDataModel<TConditionType>)conditionNodeDataModel;
            var childConditionNodes = new IConditionNode<TConditionType>[composedConditionNodeDataModel.ChildConditionNodes.Count()];
            var i = 0;

            foreach (var childConditionNodeDataModel in composedConditionNodeDataModel.ChildConditionNodes)
            {
                childConditionNodes[i++] = ConvertConditionNode(childConditionNodeDataModel);
            }

            return new ComposedConditionNode<TConditionType>(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ValueConditionNodeDataModel<TConditionType> ConvertValueConditionNode(ValueConditionNode<TConditionType> valueConditionNode)
        {
            return new ValueConditionNodeDataModel<TConditionType>
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
        private static IConditionNode<TConditionType> CreateValueConditionNode(ValueConditionNodeDataModel<TConditionType> conditionNodeDataModel)
        {
            return new ValueConditionNode<TConditionType>(
                conditionNodeDataModel.DataType,
                conditionNodeDataModel.ConditionType,
                conditionNodeDataModel.Operator,
                conditionNodeDataModel.Operand,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ConditionNodeDataModel<TConditionType> ConvertComposedConditionNode(ComposedConditionNode<TConditionType> composedConditionNode)
        {
            var conditionNodeDataModels = new ConditionNodeDataModel<TConditionType>[composedConditionNode.ChildConditionNodes.Count()];
            var i = 0;

            foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels[i++] = ConvertConditionNode(child);
            }

            return new ComposedConditionNodeDataModel<TConditionType>
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
                Properties = new PropertiesDictionary(composedConditionNode.Properties),
            };
        }

        private ConditionNodeDataModel<TConditionType> ConvertConditionNode(IConditionNode<TConditionType> conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode((ValueConditionNode<TConditionType>)conditionNode);
            }

            return ConvertComposedConditionNode((ComposedConditionNode<TConditionType>)conditionNode);
        }
    }
}