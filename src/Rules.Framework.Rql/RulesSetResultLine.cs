namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Runtime.Types;

    /// <summary>
    /// The result line type that contains one rule returned as result of a Rule Query Language
    /// source evaluation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RulesSetResultLine
    {
        internal RulesSetResultLine(int lineNumber, RqlRule rule)
        {
            this.LineNumber = lineNumber;
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber { get; }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public RqlRule Rule { get; }
    }
}