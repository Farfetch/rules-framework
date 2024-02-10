namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Runtime.Types;

    [ExcludeFromCodeCoverage]
    internal class UpdateRulePriority : UpdateRuleAttribute
    {
        private UpdateRulePriority(RqlString option, RqlAny argument)
        {
            this.Option = option;
            this.Argument = argument;
        }

        public RqlAny Argument { get; }

        public RqlString Option { get; }

        public override UpdateRuleAttributeType Type => UpdateRuleAttributeType.Priority;

        public static UpdateRulePriority Create(RqlString option, RqlAny argument) => new UpdateRulePriority(option, argument);
    }
}