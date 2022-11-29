namespace Rules.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Generics;

    internal static class GenericRuleExtensions
    {
        public static GenericConditionNode ToGenericConditionNode<TConditionType>(this IConditionNode<TConditionType> rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = rootCondition as ValueConditionNode<TConditionType>;

                return new GenericValueConditionNode
                {
                    ConditionTypeName = condition.ConditionType.ToString(),
                    DataType = condition.DataType,
                    Operand = condition.Operand,
                    Operator = condition.Operator
                };
            }

            var composedConditionNode = rootCondition as ComposedConditionNode<TConditionType>;

            var conditionNodeDataModels = new List<GenericConditionNode>(composedConditionNode.ChildConditionNodes.Count());

            foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(child.ToGenericConditionNode());
            }

            return new GenericComposedConditionNode
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator
            };
        }

        public static GenericRule ToGenericRule<TContentType, TConditionType>(this Rule<TContentType, TConditionType> rule)
        {
            return new GenericRule
            {
                RootCondition = rule.RootCondition?.ToGenericConditionNode(),
                Content = rule.ContentContainer.GetContentAs<object>(),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority
            };
        }
    }
}