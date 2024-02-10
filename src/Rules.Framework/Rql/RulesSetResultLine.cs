namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Runtime.Types;

    [ExcludeFromCodeCoverage]
    public class RulesSetResultLine<TContentType, TConditionType>
    {
        internal RulesSetResultLine(int lineNumber, RqlRule<TContentType, TConditionType> rule)
        {
            this.LineNumber = lineNumber;
            this.Rule = rule;
        }

        public int LineNumber { get; }

        public RqlRule<TContentType, TConditionType> Rule { get; }
    }
}