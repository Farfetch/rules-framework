namespace Rules.Framework.Rql
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class RulesSetResult : IResult
    {
        public RulesSetResult(string rql, int numberOfRules, IReadOnlyList<RulesSetResultLine> lines)
        {
            this.Rql = rql;
            this.NumberOfRules = numberOfRules;
            this.Lines = lines;
        }

        public IReadOnlyList<RulesSetResultLine> Lines { get; }

        public int NumberOfRules { get; }

        public string Rql { get; }
    }
}