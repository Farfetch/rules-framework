namespace Rules.Framework.WebUI.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;
    using Rules.Framework.WebUI.Dto;

    internal static class RuleDtoExtensions
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";

        public static ConditionNodeDto ToConditionNodeDto(this IConditionNode rootCondition)
        {
            /* Unmerged change from project 'Rules.Framework.WebUI (netstandard2.0)'
            Before:
                        if (rootCondition.LogicalOperator == Core.LogicalOperators.Eval ||
            After:
                        if (rootCondition.LogicalOperator == LogicalOperators.Eval ||
            */
            if (rootCondition.LogicalOperator == Framework.LogicalOperators.Eval ||
                rootCondition.LogicalOperator == 0)
            {
                var condition = rootCondition as ValueConditionNode;

                return new ValueConditionNodeDto
                {
                    ConditionTypeName = condition.ConditionType,
                    DataType = condition.DataType.ToString(),
                    Operand = condition.Operand,
                    Operator = condition.Operator.ToString(),
                };
            }

            var composedConditionNode = rootCondition as ComposedConditionNode;

            var conditionNodeDataModels = new List<ConditionNodeDto>(composedConditionNode.ChildConditionNodes.Count());

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(child.ToConditionNodeDto());
            }

            return new ComposedConditionNodeDto
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator.ToString()
            };
        }

        public static RuleDto ToRuleDto(this Rule rule, string ContentType, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer)
        {
            return new RuleDto
            {
                Conditions = rule.RootCondition?.ToConditionNodeDto(),
                ContentType = ContentType,
                Priority = rule.Priority,
                Name = rule.Name,
                Value = rule.ContentContainer.GetContentAs<object>(),
                DateEnd = !rule.DateEnd.HasValue ? null : rule.DateEnd.Value.ToString(dateFormat),
                DateBegin = rule.DateBegin.ToString(dateFormat),
                Status = !rule.Active ? RuleStatusDto.Deactivated.ToString() : ruleStatusDtoAnalyzer.Analyze(rule.DateBegin, rule.DateEnd).ToString(),
            };
        }
    }
}