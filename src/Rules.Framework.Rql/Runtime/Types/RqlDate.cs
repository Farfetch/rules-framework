namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;date&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value.ToString(\"g\"),nq}")]
    public readonly struct RqlDate : IRuntimeValue, IEquatable<RqlDate>
    {
        private static readonly Type runtimeType = typeof(DateTime);
        private static readonly RqlType type = RqlTypes.Date;

        internal RqlDate(DateTime value)
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
        public readonly DateTime Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlDate"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="rqlDate">The RQL date.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator DateTime(RqlDate rqlDate) => rqlDate.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlDate"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlDate">The RQL date.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlDate rqlDate) => new RqlAny(rqlDate);

        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="RqlDate"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlDate(DateTime value) => new RqlDate(value);

        /// <inheritdoc/>
        public bool Equals(RqlDate other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override string ToString() => $"<{Type.Name}> {this.Value:g}";
    }
}