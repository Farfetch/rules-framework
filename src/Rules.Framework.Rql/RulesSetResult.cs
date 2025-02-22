namespace Rules.Framework.Rql
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The result type that describes a set of rules returned as result of a Rule Query Language
    /// source evaluation.
    /// </summary>
    /// <seealso cref="Rules.Framework.Rql.IResult"/>
    [ExcludeFromCodeCoverage]
    public class RulesSetResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RulesSetResult"/> class.
        /// </summary>
        /// <param name="rql">The Rule Query Language source.</param>
        /// <param name="numberOfRules">The number of rules returned as result.</param>
        /// <param name="lines">The result lines.</param>
        public RulesSetResult(string rql, int numberOfRules, IReadOnlyList<RulesSetResultLine> lines)
        {
            this.Rql = rql;
            this.NumberOfRules = numberOfRules;
            this.Lines = lines;
        }

        /// <summary>
        /// Gets the result lines.
        /// </summary>
        /// <value>The result lines.</value>
        public IReadOnlyList<RulesSetResultLine> Lines { get; }

        /// <summary>
        /// Gets the number of rules returned as result.
        /// </summary>
        /// <value>The number of rules returned as result.</value>
        public int NumberOfRules { get; }

        /// <summary>
        /// Gets the Rule Query Language source.
        /// </summary>
        /// <value>The Rule Query Language source.</value>
        public string Rql { get; }
    }
}