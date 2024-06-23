namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlBool : IRuntimeValue, IEquatable<RqlBool>
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

        public static implicit operator bool(RqlBool rqlBool) => rqlBool.Value;

        public static implicit operator RqlAny(RqlBool rqlBool) => new RqlAny(rqlBool);

        public static implicit operator RqlBool(bool value) => new RqlBool(value);

        public bool Equals(RqlBool other) => this.Value == other.Value;

        public override string ToString()
                    => $"<{Type.Name}> {this.Value}";
    }
}