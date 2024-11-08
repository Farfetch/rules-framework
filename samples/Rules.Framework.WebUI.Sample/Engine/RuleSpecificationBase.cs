namespace Rules.Framework.WebUI.Sample.Engine
{
    using global::Rules.Framework;
    using global::Rules.Framework.Builder.Generic;

    internal class RuleSpecificationBase<TContentType, TConditionType>
    {
        public RuleSpecificationBase(
            RuleBuilderResult<TContentType, TConditionType> ruleBuilderResult,
            RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.RuleBuilderResult = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<TContentType, TConditionType> RuleBuilderResult { get; set; }
    }
}