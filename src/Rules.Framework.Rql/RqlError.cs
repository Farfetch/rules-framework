namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The data structure that details a Rule Query Language error.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RqlError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RqlError"/> class.
        /// </summary>
        /// <param name="text">The text that details the error.</param>
        /// <param name="rql">The Rule Query Language source to which the error referes to.</param>
        /// <param name="beginPosition">The begin position of the error.</param>
        /// <param name="endPosition">The end position of the error.</param>
        public RqlError(string text, string rql, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.Text = text;
            this.Rql = rql;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        /// <summary>
        /// Gets the begin position of the error.
        /// </summary>
        /// <value>The begin position.</value>
        public RqlSourcePosition BeginPosition { get; }

        /// <summary>
        /// Gets the end position of the error.
        /// </summary>
        /// <value>The end position.</value>
        public RqlSourcePosition EndPosition { get; }

        /// <summary>
        /// Gets the Rule Query Language source to which the error referes to.
        /// </summary>
        /// <value>The Rule Query Language source.</value>
        public string Rql { get; }

        /// <summary>
        /// Gets the text that details the error.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        /// <inheritdoc/>
        public override string ToString() => $"{this.Text} for source {this.Rql} @{this.BeginPosition}-{this.EndPosition}";
    }
}