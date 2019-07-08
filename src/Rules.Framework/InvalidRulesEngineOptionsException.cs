using System;
using System.Diagnostics.CodeAnalysis;

namespace Rules.Framework
{
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