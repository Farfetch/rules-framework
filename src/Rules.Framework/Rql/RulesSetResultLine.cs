namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Runtime.Types;

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