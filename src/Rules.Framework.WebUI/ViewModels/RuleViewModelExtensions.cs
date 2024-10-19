namespace Rules.Framework.WebUI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.WebUI.Components.PageComponents;
    using Rules.Framework.WebUI.Services;

    internal static class RuleViewModelExtensions
    {
        public static ExportRules.Rule ToExportRulesModel(this RuleViewModel ruleViewModel)
        {
            return new ExportRules.Rule
            {
                Active = ruleViewModel.Active,
                Content = ruleViewModel.Content,
                DateBegin = ruleViewModel.DateBegin,
                DateEnd = ruleViewModel.DateEnd,
                Name = ruleViewModel.Name,
                Priority = ruleViewModel.Priority,
                RootCondition = ruleViewModel.RootCondition.ToExportRulesModel(),
                Ruleset = ruleViewModel.Ruleset,
            };
        }

        public static ExportRules.ConditionNode ToExportRulesModel(this ConditionNodeViewModel rootCondition)
        {
            if (rootCondition.LogicalOperator == nameof(LogicalOperators.Eval))
            {
                var condition = rootCondition as ValueConditionNodeViewModel;

                return new ExportRules.ValueConditionNode
                {
                    Condition = condition.Condition,
                    DataType = condition.DataType.ToString(),
                    LogicalOperator = condition.LogicalOperator,
                    Operand = condition.Operand,
                    Operator = condition.Operator.ToString(),
                };
            }

            var composedConditionNode = rootCondition as ComposedConditionNodeViewModel;

            var conditionNodeDataModels = new List<ExportRules.ConditionNode>(composedConditionNode.ChildConditionNodes.Count());

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(child.ToExportRulesModel());
            }

            return new ExportRules.ComposedConditionNode
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator.ToString(),
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
                    LogicalOperator = condition.LogicalOperator.ToString(),
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
    }
}