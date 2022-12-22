namespace Rules.Framework.Source
{
    using Rules.Framework.Core;

    internal sealed class UpdateRuleArgs<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> Rule { get; set; }
    }
}