namespace Rules.Framework.Rql.Runtime
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime.Types;

    internal sealed class SearchRulesArgs
    {
        public IDictionary<string, object> Conditions { get; set; }

        public RqlDate DateBegin { get; set; }

        public RqlDate DateEnd { get; set; }

        public string Ruleset { get; set; }
    }
}