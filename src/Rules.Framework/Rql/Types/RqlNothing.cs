namespace Rules.Framework.Rql.Types
{
    using System;

    public readonly struct RqlNothing : IRuntimeValue
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Nothing;
        public Type RuntimeType => runtimeType;

        public object RuntimeValue => null;

        public RqlType Type => type;

        public override string ToString()
            => $"<{Type.Name}>";
    }
}