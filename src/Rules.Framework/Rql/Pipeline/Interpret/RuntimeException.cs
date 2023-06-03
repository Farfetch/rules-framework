namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Runtime.Serialization;
    using Rules.Framework.Rql;

    internal class RuntimeException : Exception
    {
        public RuntimeException(
            string message,
            string rql,
            RqlSourcePosition beginPosition,
            RqlSourcePosition endPosition)
            : base(message)
        {
            this.Rql = rql;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        protected RuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public string Rql { get; }
    }
}