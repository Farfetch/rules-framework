namespace Rules.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Generic;

    internal static class GenericRuleExtensions
    {
        public static GenericRule ToGenericRule<TContentType, TConditionType>(this Rule<TContentType, TConditionType> rule)
        {
            return new GenericRule
            {
                RootCondition = rule.RootCondition.ConvertConditionNode(),
                ContentContainer = rule.ContentContainer.GetContentAs<object>(),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority
            };
        }

        private static GenericConditionNode<ConditionType> ConvertConditionNode<TConditionType>(this IConditionNode<TConditionType> rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = rootCondition as ValueConditionNode<TConditionType>;

                var conditionAsEnum = Enum.Parse(typeof(TConditionType), condition.ConditionType.ToString());

                return new GenericValueConditionNode<ConditionType>
                {
                    ConditionTypeName = conditionAsEnum.ToString(),
                    DataType = condition.DataType,
                    Operand = condition.Operand,
                    Operator = condition.Operator
                };
            }

            var composedConditionNode = rootCondition as ComposedConditionNode<TConditionType>;

            var conditionNodeDataModels = new List<GenericConditionNode<ConditionType>>(composedConditionNode.ChildConditionNodes.Count());
            foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(child.ConvertConditionNode());
            }

            return new GenericComposedConditionNode<ConditionType>
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator
            };
        }
    }
}