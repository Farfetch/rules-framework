namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlObject : IRuntimeValue, IPropertyGet, IPropertySet
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Object;
        private readonly Dictionary<string, RqlAny> properties;

        public RqlObject()
        {
            this.properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal);
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => type;

        public object Value => ConvertToDictionary(this);

        public static implicit operator RqlAny(RqlObject rqlObject) => new RqlAny(rqlObject);

        public RqlAny GetPropertyValue(RqlString name) => this.properties[name.Value];

        public RqlAny SetPropertyValue(RqlString name, RqlAny value) => this.properties[name.Value] = value;

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

        private static IDictionary<string, object> ConvertToDictionary(RqlObject value)
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