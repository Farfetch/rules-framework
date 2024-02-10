namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Runtime.Types;

    [ExcludeFromCodeCoverage]
    internal class UpdateRuleDateEnd : UpdateRuleAttribute
    {
        private UpdateRuleDateEnd(RqlAny dateEnd)
        {
            this.DateEnd = dateEnd;
        }

        public RqlAny DateEnd { get; }

        public override UpdateRuleAttributeType Type => UpdateRuleAttributeType.DateEnd;

        public static UpdateRuleDateEnd Create(RqlAny dateEnd) => new UpdateRuleDateEnd(dateEnd);
    }
}