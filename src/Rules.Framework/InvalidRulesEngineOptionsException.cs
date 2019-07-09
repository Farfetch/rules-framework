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
    }
}