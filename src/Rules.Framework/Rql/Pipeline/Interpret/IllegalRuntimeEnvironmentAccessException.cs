namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;

    internal class IllegalRuntimeEnvironmentAccessException : Exception
    {
        public IllegalRuntimeEnvironmentAccessException(string message, string variableName)
            : base(message)
        {
            this.VariableName = variableName;
        }

        public string VariableName { get; }
    }
}