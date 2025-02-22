namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;read_only_object&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlReadOnlyObject : IRuntimeValue, IEquatable<RqlReadOnlyObject>
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.ReadOnlyObject;
        private readonly IDictionary<string, RqlAny> properties;

        internal RqlReadOnlyObject(IDictionary<string, RqlAny> properties)
        {
            this.properties = properties;
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
        /// Performs an implicit conversion from <see cref="RqlReadOnlyObject"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlReadOnlyObject">The RQL read only object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlReadOnlyObject rqlReadOnlyObject) => new RqlAny(rqlReadOnlyObject);

        /// <inheritdoc/>
        public bool Equals(RqlReadOnlyObject other) => this.properties.Equals(other.properties);

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