namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlType
    {
        public RqlType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            this.Name = name;
        }

        public string Name { get; }

        public static bool operator !=(RqlType left, RqlType right) => !(left == right);

        public static bool operator ==(RqlType left, RqlType right) => string.Equals(left.Name, right.Name, StringComparison.Ordinal);
    }
}