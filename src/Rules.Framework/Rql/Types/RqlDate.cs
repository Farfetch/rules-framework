namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlDate : IRuntimeValue
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

        public override string ToString()
            => $"<{Type.Name}> {this.Value}";
    }
}