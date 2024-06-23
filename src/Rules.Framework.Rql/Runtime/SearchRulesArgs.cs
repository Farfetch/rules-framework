namespace Rules.Framework.Rql.Runtime
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime.Types;

    internal sealed class SearchRulesArgs<TContentType, TConditionType>
    {
        public IEnumerable<Condition<TConditionType>> Conditions { get; set; }

        public TContentType ContentType { get; set; }

        public RqlDate DateBegin { get; set; }

        public RqlDate DateEnd { get; set; }
    }
}