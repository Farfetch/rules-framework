namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal abstract class UpdateRuleAttribute
    {
        public abstract UpdateRuleAttributeType Type { get; }
    }
}