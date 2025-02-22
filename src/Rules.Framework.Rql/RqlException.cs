namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The exception thrown when an irrecoverable error occurs evaluating Rule Query Language source.
    /// </summary>
    /// <seealso cref="System.Exception"/>
    [ExcludeFromCodeCoverage]
    public class RqlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RqlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        public RqlException(string message, RqlError error)
            : this(message, new[] { error })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        public RqlException(string message, IEnumerable<RqlError> errors)
            : base(ProcessMessage(message, errors))
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<RqlError> Errors { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(base.ToString());
            stringBuilder.AppendLine()
                .AppendLine("Errors:");
            foreach (var error in Errors)
            {
                stringBuilder.AppendFormat(
                    "---> {0} for RQL source '{1}' @ {2} to {3}",
                    error.Text,
                    error.Rql,
                    error.BeginPosition,
                    error.EndPosition);
            }

            return stringBuilder.ToString();
        }

        private static string ProcessMessage(string message, IEnumerable<RqlError> errors) => errors.Count() switch
        {
            0 => $"{message} - no error has been captured, please contact maintainers.",
            1 => $"{message} - {errors.First()}",
            _ => $"{message} - multiple errors have occurred, check exception details.",
        };
    }
}