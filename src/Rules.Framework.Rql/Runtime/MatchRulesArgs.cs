namespace Rules.Framework.Rql.Runtime
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;

    internal sealed class MatchRulesArgs
    {
        public IDictionary<string, object> Conditions { get; set; }

        public MatchCardinality MatchCardinality { get; set; }

        public RqlDate MatchDate { get; set; }

        public string Ruleset { get; set; }
    }
}