namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;bool&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value,nq}")]
    public readonly struct RqlBool : IRuntimeValue, IEquatable<RqlBool>
    {
        private static readonly Type runtimeType = typeof(bool);
        private static readonly RqlType type = RqlTypes.Bool;

        internal RqlBool(bool value)
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
        /// Gets a value indicating whether this <see cref="RqlBool"/> is value.
        /// </summary>
        /// <value><c>true</c> if value; otherwise, <c>false</c>.</value>
        public readonly bool Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlBool"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="rqlBool">The RQL bool.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(RqlBool rqlBool) => rqlBool.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlBool"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlBool">The RQL bool.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlBool rqlBool) => new RqlAny(rqlBool);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="RqlBool"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlBool(bool value) => new RqlBool(value);

        /// <inheritdoc/>
        public bool Equals(RqlBool other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override string ToString() => $"<{Type.Name}> {this.Value}";
    }
}