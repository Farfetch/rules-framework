namespace Rules.Framework.Admin.Dashboard.Sample.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RulesNotFoundException : Exception
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