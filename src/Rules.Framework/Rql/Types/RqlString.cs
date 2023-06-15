namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlString : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(string);
        private static readonly RqlType type = RqlTypes.String;

        internal RqlString(string value)
        {
            this.Value = value;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly string Value { get; }

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}