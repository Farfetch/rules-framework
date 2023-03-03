namespace Rules.Framework.IntegrationTests.Common.Features
{
    using Rules.Framework.Builder;
    using Rules.Framework.Tests.Stubs;

    public class RuleSpecification
    {
        public RuleSpecification(RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult, RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.RuleBuilderResult = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<ContentType, ConditionType> RuleBuilderResult { get; set; }
    }
}