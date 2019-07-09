namespace Rules.Framework
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InvalidRulesEngineOptionsException : Exception
    {
        public InvalidRulesEngineOptionsException(string message)
            : base(message)
        {
        }
    }
}