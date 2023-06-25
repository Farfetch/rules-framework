namespace Rules.Framework.Rql.Types
{
    public static class RqlTypes
    {
        public static RqlType Any { get; } = new RqlType("any");

        public static RqlType Array { get; } = new RqlType("array");

        public static RqlType Bool { get; } = new RqlType("bool");

        public static RqlType Date { get; } = new RqlType("date");

        public static RqlType Decimal { get; } = new RqlType("decimal");

        public static RqlType Integer { get; } = new RqlType("integer");

        public static RqlType Nothing { get; } = new RqlType("nothing");

        public static RqlType Object { get; } = new RqlType("object");

        public static RqlType String { get; } = new RqlType("string");
    }
}