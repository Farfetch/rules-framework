namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;object&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlObject : IRuntimeValue, IPropertySet, IEquatable<RqlObject>
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Object;
        private readonly Dictionary<string, RqlAny> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlObject"/> struct.
        /// </summary>
        public RqlObject()
        {
            this.properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal);
        }

        /// <inheritdoc/>
        public Type RuntimeType => runtimeType;

        /// <inheritdoc/>
        public object RuntimeValue => this.Value;

        /// <inheritdoc/>
        public RqlType Type => type;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value => ConvertToDictionary(this);

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlObject"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlObject">The RQL object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlObject rqlObject) => new RqlAny(rqlObject);

        /// <inheritdoc/>
        public bool Equals(RqlObject other) => this.properties.Equals(other.properties);

        /// <inheritdoc/>
        public RqlAny SetPropertyValue(RqlString name, RqlAny value) => this.properties[name.Value] = value;

        /// <inheritdoc/>
        public override string ToString() => $"<{Type.Name}>{Environment.NewLine}{this.ToString(4)}";

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
                    stringBuilder.Append(property.Value.Unwrap<RqlArray>().ToString(indent));
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