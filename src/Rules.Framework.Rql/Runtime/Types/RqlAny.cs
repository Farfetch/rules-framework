namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Rules.Framework.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;any&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> (<{this.UnderlyingType.Name,nq}>)")]
    public readonly struct RqlAny : IRuntimeValue, IEquatable<RqlAny>
    {
        private static readonly RqlType type = RqlTypes.Any;

        private readonly IRuntimeValue underlyingRuntimeValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlAny"/> struct.
        /// </summary>
        public RqlAny()
            : this(new RqlNothing())
        {
        }

        internal RqlAny(IRuntimeValue value)
        {
            var runtimeValue = value;
            while (runtimeValue is RqlAny rqlAny)
            {
                runtimeValue = rqlAny.Unwrap();
            }

            this.underlyingRuntimeValue = runtimeValue;
        }

        /// <inheritdoc/>
        public Type RuntimeType => this.underlyingRuntimeValue.RuntimeType;

        /// <inheritdoc/>
        public object RuntimeValue => this.underlyingRuntimeValue.RuntimeValue;

        /// <inheritdoc/>
        public RqlType Type => type;

        /// <summary>
        /// Gets the underlying RQL type under the RQL &lt;any&gt; instance.
        /// </summary>
        /// <value>The underlying RQL type under the RQL &lt;any&gt; instance.</value>
        public RqlType UnderlyingType => this.underlyingRuntimeValue.Type;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value => this.underlyingRuntimeValue.RuntimeValue;

        /// <inheritdoc/>
        public bool Equals(RqlAny other) => this.underlyingRuntimeValue == other.underlyingRuntimeValue;

        /// <inheritdoc/>
        public override string ToString()
            => $"<{this.Type.Name}> ({this.underlyingRuntimeValue.ToString()})";

        internal IRuntimeValue Unwrap() => this.underlyingRuntimeValue;

        internal T Unwrap<T>() where T : IRuntimeValue => (T)this.underlyingRuntimeValue;
    }
}