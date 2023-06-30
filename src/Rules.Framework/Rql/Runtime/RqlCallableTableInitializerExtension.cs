namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.BuiltInFunctions;

    internal static class RqlCallableTableInitializerExtension
    {
        public static RqlCallableTable Initialize(this RqlCallableTable callableTable)
        {
            var jsonToObjectFunction = new JsonToObjectFunction();
            callableTable.AddCallable(jsonToObjectFunction);
            var showFunction = new ShowFunction();
            callableTable.AddCallable(showFunction);
            return callableTable;
        }
    }
}