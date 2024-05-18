namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlDecimal : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(decimal);
        private static readonly RqlType type = RqlTypes.Decimal;

        internal RqlDecimal(decimal value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly decimal Value { get; }

        public static implicit operator decimal(RqlDecimal rqlDecimal) => rqlDecimal.Value;

        public static implicit operator RqlAny(RqlDecimal rqlDecimal) => new RqlAny(rqlDecimal);

        public static implicit operator RqlDecimal(decimal value) => new RqlDecimal(value);

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}