namespace Rules.Framework.Admin.UI.Sample.Engine
{
    using global::Rules.Framework;
    using global::Rules.Framework.Admin.UI.Sample.Enums;
    using global::Rules.Framework.Builder;

    public class RuleSpecification
    {
        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<ContentTypes, ConditionTypes> RuleBuilderResult { get; set; }
    }
}