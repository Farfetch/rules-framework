namespace Rules.Framework.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ContentTypeException : Exception
    {
        public ContentTypeException()
        {
        }

        public ContentTypeException(string message) : base(message)
        {
        }

        public ContentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}