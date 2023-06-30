namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;

    public readonly struct RqlType
    {
        private readonly IDictionary<string, RqlType> assignableTypes;

        public RqlType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            this.Name = name;
            this.assignableTypes = new Dictionary<string, RqlType>(StringComparer.Ordinal);
        }

        public IEnumerable<RqlType> AssignableTypes => this.assignableTypes.Values;

        public string Name { get; }

        public static bool operator !=(RqlType left, RqlType right) => !(left == right);

        public static bool operator ==(RqlType left, RqlType right) => string.Equals(left.Name, right.Name, StringComparison.Ordinal);

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
            string rqlTypeName = rqlType.Name;
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