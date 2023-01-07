namespace Rules.Framework.Core
{
    internal static class ConditionNodeProperties
    {
        internal static class CompilationProperties
        {
            public static string CompiledDelegateKey => $"{Prefix}_compiledDelegate";
            public static string IsCompiledKey => $"{Prefix}_isCompiled";
            public static string Prefix => "_compilation";
        }
    }
}