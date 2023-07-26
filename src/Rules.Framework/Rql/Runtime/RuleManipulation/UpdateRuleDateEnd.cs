namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using Rules.Framework.Rql.Runtime.Types;

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