namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    [ExcludeFromCodeCoverage]
    public class RqlException : Exception
    {
        public RqlException(string message, RqlError error)
            : this(message, new[] { error })
        {
        }

        public RqlException(string message, IEnumerable<RqlError> errors)
            : base(ProcessMessage(message, errors))
        {
            this.Errors = errors;
        }

        public IEnumerable<RqlError> Errors { get; }

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