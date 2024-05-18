namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class RuntimeException : Exception
    {
        public RuntimeException(string error)
            : base(error)
        {
            this.Errors = new[] { error };
        }

        public RuntimeException(IEnumerable<string> errors)
            : base(errors.Aggregate((e1, e2) => $"{e1}{Environment.NewLine}{e2}"))
        {
            this.Errors = errors;
        }

        public IEnumerable<string> Errors { get; }
    }
}