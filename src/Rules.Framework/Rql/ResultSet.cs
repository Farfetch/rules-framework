namespace Rules.Framework.Rql
{
    using System.Collections.Generic;

    public class ResultSet<TContentType, TConditionType>
    {
        public ResultSet(string rqlStatement, int affectedRules, IReadOnlyList<ResultSetLine<TContentType, TConditionType>> lines)
        {
            this.RqlStatement = rqlStatement;
            this.AffectedRules = affectedRules;
            this.Lines = lines;
        }

        public int AffectedRules { get; }

        public IReadOnlyList<ResultSetLine<TContentType, TConditionType>> Lines { get; }

        public string RqlStatement { get; }
    }
}