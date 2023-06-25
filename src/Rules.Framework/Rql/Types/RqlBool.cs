namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlBool : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(bool);
        private static readonly RqlType type = RqlTypes.Bool;

        internal RqlBool(bool value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly bool Value { get; }

        public static implicit operator RqlAny(RqlBool rqlBool) => new RqlAny(rqlBool);

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}