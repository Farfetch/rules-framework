namespace Rules.Framework.Rql
{
    using System.Collections.Generic;

    public class RulesSetResult<TContentType, TConditionType> : IResult
    {
        public RulesSetResult(string rql, int numberOfRules, IReadOnlyList<RulesSetResultLine<TContentType, TConditionType>> lines)
        {
            this.Rql = rql;
            this.NumberOfRules = numberOfRules;
            this.Lines = lines;
        }

        public IReadOnlyList<RulesSetResultLine<TContentType, TConditionType>> Lines { get; }

        public int NumberOfRules { get; }

        public string Rql { get; }
    }
}