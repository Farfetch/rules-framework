namespace Rules.Framework.Source
{
    using Rules.Framework.Core;

    internal class AddRuleArgs<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> Rule { get; set; }
    }
}
