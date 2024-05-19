namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlDate : IRuntimeValue, IEquatable<RqlDate>
    {
        private static readonly Type runtimeType = typeof(DateTime);
        private static readonly RqlType type = RqlTypes.Date;

        internal RqlDate(DateTime value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly DateTime Value { get; }

        public static implicit operator DateTime(RqlDate rqlDate) => rqlDate.Value;

        public static implicit operator RqlAny(RqlDate rqlDate) => new RqlAny(rqlDate);

        public static implicit operator RqlDate(DateTime value) => new RqlDate(value);

        public bool Equals(RqlDate other) => this.Value == other.Value;

        public override string ToString()
                    => $"<{Type.Name}> {this.Value:g}";
    }
}