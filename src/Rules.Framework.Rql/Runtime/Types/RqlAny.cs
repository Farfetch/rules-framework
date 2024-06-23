namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlAny : IRuntimeValue, IEquatable<RqlAny>
    {
        private static readonly RqlType type = RqlTypes.Any;

        private readonly IRuntimeValue underlyingRuntimeValue;

        public RqlAny()
            : this(new RqlNothing())
        {
        }

        internal RqlAny(IRuntimeValue value)
        {
            var runtimeValue = value;
            while (runtimeValue is RqlAny rqlAny)
            {
                runtimeValue = rqlAny.Unwrap();
            }

            this.underlyingRuntimeValue = runtimeValue;
        }

        public Type RuntimeType => this.underlyingRuntimeValue.RuntimeType;

        public object RuntimeValue => this.underlyingRuntimeValue.RuntimeValue;

        public RqlType Type => type;

        public RqlType UnderlyingType => this.underlyingRuntimeValue.Type;

        public object Value => this.underlyingRuntimeValue.RuntimeValue;

        public bool Equals(RqlAny other) => this.underlyingRuntimeValue == other.underlyingRuntimeValue;

        public override string ToString()
            => $"<{this.Type.Name}> ({this.underlyingRuntimeValue.ToString()})";

        internal IRuntimeValue Unwrap() => this.underlyingRuntimeValue;

        internal T Unwrap<T>() where T : IRuntimeValue => (T)this.underlyingRuntimeValue;
    }
}