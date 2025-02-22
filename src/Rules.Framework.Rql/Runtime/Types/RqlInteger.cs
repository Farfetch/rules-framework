namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;integer&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value,nq}")]
    public readonly struct RqlInteger : IRuntimeValue, IEquatable<RqlInteger>
    {
        private static readonly Type runtimeType = typeof(int);
        private static readonly RqlType type = RqlTypes.Integer;

        internal RqlInteger(int value)
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
        public readonly int Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlInteger"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="rqlInteger">The RQL integer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(RqlInteger rqlInteger) => rqlInteger.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlInteger"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlInteger">The RQL integer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlInteger rqlInteger) => new RqlAny(rqlInteger);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="RqlInteger"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlInteger(int value) => new RqlInteger(value);

        /// <inheritdoc/>
        public bool Equals(RqlInteger other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override string ToString()
                    => $"<{Type.Name}> {this.Value}";
    }
}