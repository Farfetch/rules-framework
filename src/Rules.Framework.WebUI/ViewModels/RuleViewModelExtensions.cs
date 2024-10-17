namespace Rules.Framework.WebUI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.WebUI.Services;

    internal static class RuleViewModelExtensions
    {
        public static ConditionNodeViewModel ToViewModel(this IConditionNode rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval ||
                rootCondition.LogicalOperator == 0)
            {
                var condition = rootCondition as ValueConditionNode;

                return new ValueConditionNodeViewModel
                {
                    Condition = condition.Condition,
                    DataType = condition.DataType.ToString(),
                    Operand = condition.Operand,
                    Operator = condition.Operator.ToString(),
                };
            }

            var composedConditionNode = rootCondition as ComposedConditionNode;

            var conditionNodeDataModels = new List<ConditionNodeViewModel>(composedConditionNode.ChildConditionNodes.Count());

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(child.ToViewModel());
            }

            return new ComposedConditionNodeViewModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator.ToString()
            };
        }

        public static RuleViewModel ToViewModel(this Rule rule)
        {
            return new RuleViewModel
            {
                Active = rule.Active,
                Content = rule.ContentContainer.GetContentAs<object>(),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Id = GuidGenerator.GenerateFromString(rule.Name),
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition.ToViewModel(),
                Ruleset = rule.Ruleset,
            };
        }
    }
}