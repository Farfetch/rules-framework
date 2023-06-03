namespace Rules.Framework.Rql
{
    using Rules.Framework.Core;

    public class ResultSetLine<TContentType, TConditionType>
    {
        internal ResultSetLine(int lineNumber, Rule<TContentType, TConditionType> rule)
        {
            this.LineNumber = lineNumber;
            this.Rule = rule;
        }

        public int LineNumber { get; }

        public Rule<TContentType, TConditionType> Rule { get; }
    }
}