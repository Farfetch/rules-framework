namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Runtime.Serialization;

    internal class InterpreterException : Exception
    {
        public InterpreterException(
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

        protected InterpreterException(SerializationInfo info, StreamingContext context)
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