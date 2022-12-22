namespace Rules.Framework.Source
{
    using Rules.Framework.Core;

    internal sealed class AddRuleArgs<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> Rule { get; set; }
    }
}