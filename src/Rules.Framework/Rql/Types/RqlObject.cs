namespace Rules.Framework.Rql.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public readonly struct RqlObject : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Object;

        public RqlObject()
        {
            this.Properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal);
        }

        public IDictionary<string, RqlAny> Properties { get; }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public object Value => ConvertToDictionary(this);

        public static implicit operator RqlAny(RqlObject rqlObject) => new RqlAny(rqlObject);

        public override string ToString()
            => $"<{Type.Name}>{Environment.NewLine}{this.ToString(4)}";

        internal string ToString(int indent)
        {
            var stringBuilder = new StringBuilder()
                .Append('{');

            foreach (var property in this.Properties)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indent))
                    .Append(property.Key)
                    .Append(": ")
                    .Append(property.Value.UnderlyingType == RqlTypes.Object
                        ? property.Value.Unwrap<RqlObject>().ToString(indent + 4)
                        : property.Value.Value);
            }

            return stringBuilder.AppendLine()
                .Append(new string(' ', indent - 4))
                .Append('}')
                .ToString();
        }

        private static IDictionary<string, object> ConvertToDictionary(RqlObject value)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kvp in value.Properties)
            {
                result[kvp.Key] = kvp.Value.RuntimeValue;
            }

            return result;
        }
    }
}