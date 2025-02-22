namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;type&gt; value.
    /// </summary>
    [DebuggerDisplay("RQL Type: {this.Name,nq}")]
    public readonly struct RqlType : IEquatable<RqlType>
    {
        private readonly IDictionary<string, RqlType> assignableTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlType"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException">
        /// '{nameof(name)}' cannot be null or whitespace. - name
        /// </exception>
        public RqlType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            this.Name = name;
            this.assignableTypes = new Dictionary<string, RqlType>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the assignable RQL types.
        /// </summary>
        /// <value>The assignable types.</value>
        public IEnumerable<RqlType> AssignableTypes => this.assignableTypes.Values;

        /// <summary>
        /// Gets the RQL type name.
        /// </summary>
        /// <value>The RQL type name.</value>
        public string Name { get; }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RqlType left, RqlType right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RqlType left, RqlType right) => string.Equals(left.Name, right.Name, StringComparison.Ordinal);

        /// <inheritdoc/>
        public bool Equals(RqlType other) => string.Equals(this.Name, other.Name, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null || obj is not RqlType)
            {
                return false;
            }

            return this.Equals((RqlType)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.assignableTypes, this.AssignableTypes, this.Name);

        /// <summary>
        /// Determines whether the RQL type represented by this is instance is assignable to given
        /// RQL type.
        /// </summary>
        /// <param name="rqlType">the RQL type to test.</param>
        /// <returns>
        /// <c>true</c> if the RQL type represented by this is instance is assignable to given RQL
        /// type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAssignableTo(RqlType rqlType)
        {
            if (string.Equals(rqlType.Name, this.Name, StringComparison.Ordinal))
            {
                return true;
            }

            return this.assignableTypes.ContainsKey(rqlType.Name);
        }

        internal void AddAssignableType(RqlType rqlType)
        {
            var rqlTypeName = rqlType.Name;
            if (string.Equals(rqlTypeName, this.Name, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Type already is assignable to itself.");
            }

            if (this.assignableTypes.ContainsKey(rqlTypeName))
            {
                throw new InvalidOperationException($"Assignable type '{rqlType.Name}' has already been added to {this.Name}.");
            }

            this.assignableTypes[rqlTypeName] = rqlType;
        }
    }
}