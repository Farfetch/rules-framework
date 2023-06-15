namespace Rules.Framework.Rql.Types
{
    using System;

    internal readonly struct Parameter
    {
        public Parameter(RqlType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            this.Type = type;
            this.Name = name;
        }

        public string Name { get; }

        public RqlType Type { get; }

        public override string ToString() => $"<{this.Type.Name}> {this.Name}";
    }
}