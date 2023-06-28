namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public readonly struct RqlReadOnlyObject : IRuntimeValue, IPropertyGet
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.ReadOnlyObject;
        private readonly IDictionary<string, RqlAny> properties;

        internal RqlReadOnlyObject(IDictionary<string, RqlAny> properties)
        {
            this.properties = properties;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public object Value => ConvertToDictionary(this);

        public static implicit operator RqlAny(RqlReadOnlyObject rqlReadOnlyObject) => new RqlAny(rqlReadOnlyObject);

        public RqlAny GetPropertyValue(RqlString name) => this.properties[name.Value];

        public override string ToString()
            => $"<{Type.Name}>{Environment.NewLine}{this.ToString(4)}";

        public RqlBool TryGetPropertyValue(RqlString memberName, out RqlAny result) => this.properties.TryGetValue(memberName.Value, out result);

        internal string ToString(int indent)
        {
            var stringBuilder = new StringBuilder()
                .Append('{');

            foreach (var property in this.properties)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indent))
                    .Append(property.Key)
                    .Append(": ");

                if (property.Value.UnderlyingType == RqlTypes.Object)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlObject>().ToString(indent + 4));
                    continue;
                }

                if (property.Value.UnderlyingType == RqlTypes.ReadOnlyObject)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlReadOnlyObject>().ToString(indent + 4));
                    continue;
                }

                if (property.Value.UnderlyingType == RqlTypes.Array)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlArray>().ToString());
                    continue;
                }

                stringBuilder.Append(property.Value.Value);
            }

            return stringBuilder.AppendLine()
                .Append(new string(' ', indent - 4))
                .Append('}')
                .ToString();
        }

        private static IDictionary<string, object> ConvertToDictionary(RqlReadOnlyObject value)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kvp in value.properties)
            {
                result[kvp.Key] = kvp.Value.RuntimeValue;
            }

            return result;
        }
    }
}