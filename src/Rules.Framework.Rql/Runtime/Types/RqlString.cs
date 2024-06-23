namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlString : IRuntimeValue, IEquatable<RqlString>
    {
        private static readonly Type runtimeType = typeof(string);
        private static readonly RqlType type = RqlTypes.String;

        internal RqlString(string value)
        {
            this.Value = value ?? string.Empty;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public readonly string Value { get; }

        public static implicit operator RqlAny(RqlString rqlString) => new RqlAny(rqlString);

        public static implicit operator RqlString(string value) => new RqlString(value);

        public static implicit operator string(RqlString rqlString) => rqlString.Value;

        public bool Equals(RqlString other) => this.Value == other.Value;

        public override string ToString()
                            => @$"<{Type.Name}> ""{this.Value}""";
    }
}