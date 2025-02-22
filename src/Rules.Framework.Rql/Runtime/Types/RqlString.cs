namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;string&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value}")]
    public readonly struct RqlString : IRuntimeValue, IEquatable<RqlString>
    {
        private static readonly Type runtimeType = typeof(string);
        private static readonly RqlType type = RqlTypes.String;

        internal RqlString(string value)
        {
            this.Value = value ?? string.Empty;
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
        public readonly string Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlString"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlString">The RQL string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlString rqlString) => new RqlAny(rqlString);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="RqlString"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlString(string value) => new RqlString(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlString"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="rqlString">The RQL string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(RqlString rqlString) => rqlString.Value;

        /// <inheritdoc/>
        public bool Equals(RqlString other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override string ToString() => @$"<{Type.Name}> ""{this.Value}""";
    }
}