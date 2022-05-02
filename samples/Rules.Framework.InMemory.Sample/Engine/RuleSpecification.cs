namespace Rules.Framework.InMemory.Sample.Engine
{
    using global::Rules.Framework;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.InMemory.Sample.Enums;

    internal class RuleSpecification
    {
        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<ContentTypes, ConditionTypes> RuleBuilderResult { get; set; }
    }
}