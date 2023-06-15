namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlObject : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Object;

        internal RqlObject(object value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public object Value { get; }

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}