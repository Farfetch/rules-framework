namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;

    public class RqlException : Exception
    {
        public RqlException(string message, RqlError error)
            : this(message, new[] { error })
        {
        }

        public RqlException(string message, IEnumerable<RqlError> errors)
            : base(message)
        {
            this.Errors = errors;
        }

        public IEnumerable<RqlError> Errors { get; }
    }
}