namespace Rules.Framework.InMemory.Sample.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class RulesNotFoundException : Exception
    {
        public RulesNotFoundException()
        {
        }

        public RulesNotFoundException(string message) : base(message)
        {
        }

        public RulesNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RulesNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}