namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The result type that describes a value returned as result of a Rule Query Language source evaluation.
    /// </summary>
    /// <seealso cref="Rules.Framework.Rql.IResult"/>
    [ExcludeFromCodeCoverage]
    public class ValueResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueResult"/> class.
        /// </summary>
        /// <param name="rql">The Rule Query Language source.</param>
        /// <param name="value">The value.</param>
        public ValueResult(string rql, object value)
        {
            this.Rql = rql;
            this.Value = value;
        }

        /// <summary>
        /// Gets the Rule Query Language source.
        /// </summary>
        /// <value>The Rule Query Language source.</value>
        public string Rql { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; }
    }
}