namespace Rules.Framework.Rql.Runtime.Types
{
    public static class RqlTypes
    {
        static RqlTypes()
        {
            // Types bootstrap.
            Any = new RqlType("any");
            Array = new RqlType("array");
            Bool = new RqlType("bool");
            Date = new RqlType("date");
            Decimal = new RqlType("decimal");
            Integer = new RqlType("integer");
            Nothing = new RqlType("nothing");
            Object = new RqlType("object");
            ReadOnlyObject = new RqlType("read_only_object");
            Rule = new RqlType("rule");
            String = new RqlType("string");

            // Register assignables.
            Array.AddAssignableType(Any);
            Bool.AddAssignableType(Any);
            Date.AddAssignableType(Any);
            Decimal.AddAssignableType(Any);
            Integer.AddAssignableType(Any);
            Nothing.AddAssignableType(Any);
            Object.AddAssignableType(Any);
            ReadOnlyObject.AddAssignableType(Any);
            Rule.AddAssignableType(Any);
            String.AddAssignableType(Any);
        }

        public static RqlType Any { get; }

        public static RqlType Array { get; }

        public static RqlType Bool { get; }

        public static RqlType Date { get; }

        public static RqlType Decimal { get; }

        public static RqlType Integer { get; }

        public static RqlType Nothing { get; }

        public static RqlType Object { get; }

        public static RqlType ReadOnlyObject { get; }

        public static RqlType Rule { get; }

        public static RqlType String { get; }
    }
}