namespace Rules.Framework.Rql.Types
{
    using System;

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

        public static implicit operator RqlAny(RqlDecimal rqlDecimal) => new RqlAny(rqlDecimal);

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}