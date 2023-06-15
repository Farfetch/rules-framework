namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlAny : IRuntimeValue
    {
        private static readonly RqlType type = RqlTypes.Any;

        private readonly IRuntimeValue underlyingRuntimeValue;

        internal RqlAny(IRuntimeValue value)
        {
            this.underlyingRuntimeValue = value;
        }

        public Type RuntimeType => this.underlyingRuntimeValue.RuntimeType;

        public object RuntimeValue => this.underlyingRuntimeValue.RuntimeValue;

        public RqlType Type => type;

        public RqlType UnderlyingType => this.underlyingRuntimeValue.Type;

        public object Value => this.underlyingRuntimeValue.RuntimeValue;

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";

        internal T Unwrap<T>() where T : IRuntimeValue => (T)this.underlyingRuntimeValue;
    }
}