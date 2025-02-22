namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;decimal&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value,nq}")]
    public readonly struct RqlDecimal : IRuntimeValue, IEquatable<RqlDecimal>
    {
        private static readonly Type runtimeType = typeof(decimal);
        private static readonly RqlType type = RqlTypes.Decimal;

        internal RqlDecimal(decimal value)
        {
            this.Value = value;
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
        public readonly decimal Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlDecimal"/> to <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="rqlDecimal">The RQL decimal.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator decimal(RqlDecimal rqlDecimal) => rqlDecimal.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlDecimal"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlDecimal">The RQL decimal.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlDecimal rqlDecimal) => new RqlAny(rqlDecimal);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Decimal"/> to <see cref="RqlDecimal"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlDecimal(decimal value) => new RqlDecimal(value);

        /// <inheritdoc/>
        public bool Equals(RqlDecimal other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override string ToString() => $"<{Type.Name}> {this.Value}";
    }
}