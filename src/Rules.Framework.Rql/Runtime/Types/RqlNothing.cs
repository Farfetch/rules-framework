namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlNothing : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Nothing;
        public Type RuntimeType => runtimeType;

        public object RuntimeValue => null;

        public RqlType Type => type;

        public static implicit operator RqlAny(RqlNothing rqlNothing) => new RqlAny(rqlNothing);

        public override string ToString()
            => $"<{Type.Name}>";
    }
}