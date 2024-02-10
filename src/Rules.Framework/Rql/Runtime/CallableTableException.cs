namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class CallableTableException : Exception
    {
        public CallableTableException(string message, string callableSpace, string callableName, string[] callableParameterTypes)
            : base(message)
        {
            this.CallableSpace = callableSpace;
            this.CallableName = callableName;
            this.CallableParameterTypes = callableParameterTypes;
        }

        public string CallableName { get; }

        public string[] CallableParameterTypes { get; }

        public string CallableSpace { get; }
    }
}