namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Runtime.Types;

    [ExcludeFromCodeCoverage]
    public class RulesSetResultLine
    {
        internal RulesSetResultLine(int lineNumber, RqlRule rule)
        {
            this.LineNumber = lineNumber;
            this.Rule = rule;
        }

        public int LineNumber { get; }

        public RqlRule Rule { get; }
    }
}