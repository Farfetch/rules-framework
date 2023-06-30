namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.BuiltInFunctions;
    using Rules.Framework.Rql.Runtime.Types;

    internal static class RqlCallableTableInitializerExtension
    {
        public static RqlCallableTable Initialize(this RqlCallableTable callableTable)
        {
            var jsonToObjectFunction = new JsonToObjectFunction();
            callableTable.AddCallable(jsonToObjectFunction);
            var showFunction = new ShowFunction();
            callableTable.AddCallable(showFunction);
            var toStringParameterlessFunction = new ToStringParameterlessFunction();
            callableTable.AddCallable(RqlTypes.Any, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Array, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Bool, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Date, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Decimal, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Integer, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Nothing, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Object, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.ReadOnlyObject, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.Rule, toStringParameterlessFunction);
            callableTable.AddCallable(RqlTypes.String, toStringParameterlessFunction);
            return callableTable;
        }
    }
}