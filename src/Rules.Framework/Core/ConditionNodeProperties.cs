namespace Rules.Framework.Core
{
    using System;

    internal static class ConditionNodeProperties
    {
        public static string CompilationPrefix => "_compilation";

        public static string CompiledFlagKey => $"{CompilationPrefix}_isCompiled";

        public static string GetCompiledDelegateKey(string multiplicity)
        {
            if (string.IsNullOrWhiteSpace(multiplicity))
            {
                throw new ArgumentException($"'{nameof(multiplicity)}' cannot be null or whitespace.", nameof(multiplicity));
            }

            return $"{CompilationPrefix}_compiled_{multiplicity}";
        }
    }
}
