namespace Rules.Framework.WebUI.Sample.Engine
{
    using global::Rules.Framework;
    using global::Rules.Framework.Builder.Generic;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal sealed class RuleSpecification : RuleSpecificationBase<ContentTypes, ConditionTypes>
    {
        public RuleSpecification(
            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult,
            RuleAddPriorityOption ruleAddPriorityOption)
            : base(ruleBuilderResult, ruleAddPriorityOption)
        {
        }
    }
}