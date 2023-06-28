namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlString : IRuntimeValue, IPropertyGet
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

        public static implicit operator RqlAny(RqlString rqlString) => new RqlAny(rqlString);

        public static implicit operator RqlString(string value) => new RqlString(value);

        public static implicit operator string(RqlString rqlString) => rqlString.Value;

        public RqlAny GetPropertyValue(RqlString name)
        {
            if (this.TryGetPropertyValue(name, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException($"Key with name '{name.Value}' has not been found.");
        }

        public override string ToString()
                    => $"<{Type.Name}> {this.Value}";

        public RqlBool TryGetPropertyValue(RqlString memberName, out RqlAny result)
        {
            if (string.Equals(memberName, "Size", StringComparison.Ordinal))
            {
                result = new RqlInteger(this.Value?.Length ?? 0);
                return true;
            }

            result = new RqlNothing();
            return false;
        }
    }
}