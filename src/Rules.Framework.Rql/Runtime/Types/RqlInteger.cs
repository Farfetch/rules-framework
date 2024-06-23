namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlInteger : IRuntimeValue, IEquatable<RqlInteger>
    {
        private static readonly Type runtimeType = typeof(int);
        private static readonly RqlType type = RqlTypes.Integer;

        internal RqlInteger(int value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly int Value { get; }

        public static implicit operator int(RqlInteger rqlInteger) => rqlInteger.Value;

        public static implicit operator RqlAny(RqlInteger rqlInteger) => new RqlAny(rqlInteger);

        public static implicit operator RqlInteger(int value) => new RqlInteger(value);

        public bool Equals(RqlInteger other) => this.Value == other.Value;

        public override string ToString()
                    => $"<{Type.Name}> {this.Value}";
    }
}