namespace Rules.Framework.Rql.Runtime.Types
{
    /// <summary>
    /// Provides access to all defined Rule Query Language types.
    /// </summary>
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
            Ruleset = new RqlType("ruleset");
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
            Ruleset.AddAssignableType(Any);
            String.AddAssignableType(Any);
        }

        /// <summary>
        /// Gets the RQL type for &lt;any&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;any&gt;.</value>
        public static RqlType Any { get; }

        /// <summary>
        /// Gets the RQL type for &lt;array&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;array&gt;.</value>
        public static RqlType Array { get; }

        /// <summary>
        /// Gets the RQL type for &lt;bool&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;bool&gt;.</value>
        public static RqlType Bool { get; }

        /// <summary>
        /// Gets the RQL type for &lt;date&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;date&gt;.</value>
        public static RqlType Date { get; }

        /// <summary>
        /// Gets the RQL type for &lt;decimal&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;decimal&gt;.</value>
        public static RqlType Decimal { get; }

        /// <summary>
        /// Gets the RQL type for &lt;integer&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;integer&gt;.</value>
        public static RqlType Integer { get; }

        /// <summary>
        /// Gets the RQL type for &lt;nothing&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;nothing&gt;.</value>
        public static RqlType Nothing { get; }

        /// <summary>
        /// Gets the RQL type for &lt;object&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;object&gt;.</value>
        public static RqlType Object { get; }

        /// <summary>
        /// Gets the RQL type for &lt;read_only_object&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;read_only_object&gt;.</value>
        public static RqlType ReadOnlyObject { get; }

        /// <summary>
        /// Gets the RQL type for &lt;rule&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;rule&gt;.</value>
        public static RqlType Rule { get; }

        /// <summary>
        /// Gets the RQL type for &lt;ruleset&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;ruleset&gt;.</value>
        public static RqlType Ruleset { get; }

        /// <summary>
        /// Gets the RQL type for &lt;string&gt;.
        /// </summary>
        /// <value>The RQL type for &lt;string&gt;.</value>
        public static RqlType String { get; }
    }
}