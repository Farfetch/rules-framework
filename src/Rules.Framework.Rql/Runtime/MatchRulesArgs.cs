namespace Rules.Framework.Rql.Runtime
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;

    internal sealed class MatchRulesArgs<TContentType, TConditionType>
    {
        public IEnumerable<Condition<TConditionType>> Conditions { get; set; }

        public TContentType ContentType { get; set; }

        public MatchCardinality MatchCardinality { get; set; }

        public RqlDate MatchDate { get; set; }
    }
}