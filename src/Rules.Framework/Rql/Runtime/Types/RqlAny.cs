namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlAny : IRuntimeValue
    {
        private static readonly RqlType type = RqlTypes.Any;

        private readonly IRuntimeValue underlyingRuntimeValue;

        internal RqlAny(IRuntimeValue value)
        {
            var underlyingRuntimeValue = value;
            while (underlyingRuntimeValue is RqlAny rqlAny)
            {
                underlyingRuntimeValue = rqlAny.Unwrap();
            }

            this.underlyingRuntimeValue = underlyingRuntimeValue;
        }

        public Type RuntimeType => this.underlyingRuntimeValue.RuntimeType;

        public object RuntimeValue => this.underlyingRuntimeValue.RuntimeValue;

        public RqlType Type => type;

        public RqlType UnderlyingType => this.underlyingRuntimeValue.Type;

        public object Value => this.underlyingRuntimeValue.RuntimeValue;

        public override string ToString()
            => $"<{this.Type.Name}> ({this.underlyingRuntimeValue.ToString()})";

        internal IRuntimeValue Unwrap() => this.underlyingRuntimeValue;

        internal T Unwrap<T>() where T : IRuntimeValue => (T)this.underlyingRuntimeValue;
    }
}