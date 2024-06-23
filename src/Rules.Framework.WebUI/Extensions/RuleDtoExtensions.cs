namespace Rules.Framework.WebUI.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.WebUI.Dto;

    internal static class RuleDtoExtensions
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";

        public static ConditionNodeDto ToConditionNodeDto(this IConditionNode<string> rootCondition)
        {
            if (rootCondition.LogicalOperator == Core.LogicalOperators.Eval ||
                rootCondition.LogicalOperator == 0)
            {
                var condition = (ValueConditionNode<string>)rootCondition;

                return new ValueConditionNodeDto
                {
                    ConditionTypeName = condition.ConditionType,
                    DataType = condition.DataType.ToString(),
                    Operand = condition.Operand,
                    Operator = condition.Operator.ToString(),
                };
            }

            var composedConditionNode = (ComposedConditionNode<string>)rootCondition;

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

        public static RuleDto ToRuleDto(this Rule<string, string> rule, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer)
        {
            return new RuleDto
            {
                Conditions = rule.RootCondition?.ToConditionNodeDto(),
                ContentType = rule.ContentContainer.ContentType,
                Priority = rule.Priority,
                Name = rule.Name,
                Value = rule.ContentContainer.GetContentAs<dynamic>(),
                DateEnd = !rule.DateEnd.HasValue ? null : rule.DateEnd.Value.ToString(dateFormat),
                DateBegin = rule.DateBegin.ToString(dateFormat),
                Status = !rule.Active ? RuleStatusDto.Deactivated.ToString() : ruleStatusDtoAnalyzer.Analyze(rule.DateBegin, rule.DateEnd).ToString(),
            };
        }
    }
}