namespace Rules.Framework.IntegrationTests.Common.Features
{
    using Rules.Framework.Core;
    using Rules.Framework.Tests.Stubs;

    public class RuleSpecification
    {
        public RuleSpecification(Rule<ContentType, ConditionType> ruleBuilderResult, RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.Rule = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public Rule<ContentType, ConditionType> Rule { get; set; }
    }
}