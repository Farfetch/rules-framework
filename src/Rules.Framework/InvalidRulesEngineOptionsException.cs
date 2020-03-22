namespace Rules.Framework
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A exception thrown when invalid rules engine options are specified.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InvalidRulesEngineOptionsException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="InvalidRulesEngineOptionsException"/>.
        /// </summary>
        /// <param name="message">the message.</param>
        public InvalidRulesEngineOptionsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRulesEngineOptionsException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected InvalidRulesEngineOptionsException(
            System.Runtime.Serialization.SerializationInfo serializationInfo,
            System.Runtime.Serialization.StreamingContext streamingContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRulesEngineOptionsException"/> class.
        /// </summary>
        public InvalidRulesEngineOptionsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRulesEngineOptionsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidRulesEngineOptionsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}