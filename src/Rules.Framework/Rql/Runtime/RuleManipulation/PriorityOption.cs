namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using System.Runtime.InteropServices;
    using Rules.Framework.Rql.Runtime.Types;

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct PriorityOption
    {
        public PriorityOption(RqlString option, RqlAny argument)
        {
            this.Option = option;
            this.Argument = argument;
            this.IsEmpty = string.IsNullOrEmpty(option) && argument.UnderlyingType == RqlTypes.Nothing;
        }

        public RqlAny Argument { get; }

        public RqlBool IsEmpty { get; }

        public RqlString Option { get; }

        public static PriorityOption None { get; } = new PriorityOption(string.Empty, new RqlNothing());
    }
}