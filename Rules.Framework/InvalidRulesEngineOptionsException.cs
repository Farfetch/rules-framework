using System;

namespace Rules.Framework
{
    public class InvalidRulesEngineOptionsException : Exception
    {
        public InvalidRulesEngineOptionsException(string message)
            : base(message)
        {
        }
    }
}