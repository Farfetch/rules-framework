namespace Rules.Framework.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a content type exception thrown when a content type is unable to be processed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ContentTypeException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ContentTypeException"/>.
        /// </summary>
        public ContentTypeException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="ContentTypeException"/>.
        /// </summary>
        /// <param name="message">the message.</param>
        public ContentTypeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ContentTypeException"/>.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerException">the inner exception.</param>
        public ContentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ContentTypeException"/>.
        /// </summary>
        /// <param name="info">the serialization info</param>
        /// <param name="context">the streaming context.</param>
        protected ContentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}