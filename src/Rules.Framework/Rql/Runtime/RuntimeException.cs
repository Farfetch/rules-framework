namespace Rules.Framework.Rql.Runtime
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

        protected RuntimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Rql = info.GetString(nameof(this.Rql));
            this.BeginPosition = (RqlSourcePosition)info.GetValue(nameof(this.BeginPosition), typeof(RqlSourcePosition));
            this.EndPosition = (RqlSourcePosition)info.GetValue(nameof(this.EndPosition), typeof(RqlSourcePosition));
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public string Rql { get; }
    }
}