namespace Rules.Framework.Generics
{
    using System.Collections.Generic;

    public class GenericRulesSetRqlResult : IGenericRqlResult
    {
        public GenericRulesSetRqlResult(string rql, int numberOfRules, IReadOnlyList<GenericRulesSetRqlResultLine> lines)
        {
            this.Rql = rql;
            this.NumberOfRules = numberOfRules;
            this.Lines = lines;
        }

        public IReadOnlyList<GenericRulesSetRqlResultLine> Lines { get; }

        public int NumberOfRules { get; }

        public string Rql { get; }
    }
}