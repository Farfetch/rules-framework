namespace Rules.Framework.Admin.Dashboard.Sample.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RulesBuilderException : Exception
    {
        public RulesBuilderException()
        {
        }

        public RulesBuilderException(string message) : base(message)
        {
        }

        public RulesBuilderException(string message, IEnumerable<string> ruleEngineErrors) : base(message)
        {
            this.RuleEngineErrors = ruleEngineErrors;
        }

        public RulesBuilderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RulesBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<string> RuleEngineErrors { get; }
    }
}