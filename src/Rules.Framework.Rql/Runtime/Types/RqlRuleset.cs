namespace Rules.Framework.Rql.Runtime.Types
{
    using System;

    public readonly struct RqlRuleset : IRuntimeValue, IEquatable<RqlRuleset>
    {
        private static readonly Type runtimeType = typeof(Ruleset);

        public RqlRuleset(Ruleset ruleset)
        {
            this.Value = ruleset;
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => RqlTypes.Ruleset;

        public readonly Ruleset Value { get; }

        public static implicit operator RqlAny(RqlRuleset rqlRuleset) => new RqlAny(rqlRuleset);

        public bool Equals(RqlRuleset other) => this.Value.Equals(other.Value);

        public override string ToString()
            => $"<{Type.Name}> {this.Value.Name}";
    }
}