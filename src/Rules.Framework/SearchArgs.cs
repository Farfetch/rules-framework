namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;

    public class SearchArgs<TContentType, TConditionType>
    {
        public IEnumerable<Condition<TConditionType>> Conditions { get; set; }

        public TContentType ContentType { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public bool ExcludeRulesWithoutSearchConditions { get; set; }
    }
}