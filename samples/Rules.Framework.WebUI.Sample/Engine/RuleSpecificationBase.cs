namespace Rules.Framework.WebUI.Sample.Engine
{
    using global::Rules.Framework;
    using global::Rules.Framework.Builder.Generic;

    internal class RuleSpecificationBase<TRuleset, TCondition>
    {
        public RuleSpecificationBase(
            RuleBuilderResult<TRuleset, TCondition> ruleBuilderResult,
            RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.RuleBuilderResult = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<TRuleset, TCondition> RuleBuilderResult { get; set; }
    }
}