namespace Rules.Framework.Rql
{
    using System.Collections.Generic;

    public class RulesSetResult<TContentType, TConditionType> : IResult
    {
        public RulesSetResult(string rql, int affectedRules, IReadOnlyList<RulesSetResultLine<TContentType, TConditionType>> lines)
        {
            this.Rql = rql;
            this.AffectedRules = affectedRules;
            this.Lines = lines;
        }

        public int AffectedRules { get; }

        public IReadOnlyList<RulesSetResultLine<TContentType, TConditionType>> Lines { get; }

        public string Rql { get; }
    }
}