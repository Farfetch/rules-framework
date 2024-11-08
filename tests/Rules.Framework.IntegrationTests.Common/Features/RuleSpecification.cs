namespace Rules.Framework.IntegrationTests.Common.Features
{
    using Rules.Framework.Generic;
    using Rules.Framework.Tests.Stubs;

    public class RuleSpecification
    {
        public RuleSpecification(Rule<RulesetNames, ConditionNames> ruleBuilderResult, RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.Rule = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public Rule<RulesetNames, ConditionNames> Rule { get; set; }
        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }
    }
}