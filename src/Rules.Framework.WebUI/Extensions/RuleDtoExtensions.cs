namespace Rules.Framework.WebUI.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;

    internal static class RuleDtoExtensions
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";

        public static ConditionNodeDto ToConditionNodeDto(this GenericConditionNode rootCondition)
        {
            if (rootCondition.LogicalOperator == Core.LogicalOperators.Eval ||
                rootCondition.LogicalOperator == 0)
            {
                var condition = rootCondition as GenericValueConditionNode;

                return new ValueConditionNodeDto
                {
                    ConditionTypeName = condition.ConditionTypeName,
                    DataType = condition.DataType.ToString(),
                    Operand = condition.Operand.ToString(),
                    Operator = condition.Operator.ToString()
                };
            }

            var composedConditionNode = rootCondition as GenericComposedConditionNode;

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

        public static RuleDto ToRuleDto(this GenericRule rule, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer)
        {
            return new RuleDto
            {
                Conditions = rule.RootCondition?.ToConditionNodeDto(),
                Priority = rule.Priority,
                Name = rule.Name,
                Value = rule.Content,
                DateEnd = !rule.DateEnd.HasValue ? null : rule.DateEnd.Value.ToString(dateFormat),
                DateBegin = rule.DateBegin.ToString(dateFormat),
                Status = ruleStatusDtoAnalyzer.Analyze(rule.DateBegin, rule.DateEnd).ToString(),
            };
        }
    }
}