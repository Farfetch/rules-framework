namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.BuiltInFunctions;

    internal static class RuntimeEnvironmentInitializerExtension
    {
        public static IRuntimeEnvironment Initialize(this IRuntimeEnvironment runtimeEnvironment)
        {
            var jsonToObjectFunction = new JsonToObjectFunction();
            runtimeEnvironment.Define(jsonToObjectFunction.Name, jsonToObjectFunction);
            var showFunction = new ShowFunction();
            runtimeEnvironment.Define(showFunction.Name, showFunction);
            return runtimeEnvironment;
        }
    }
}