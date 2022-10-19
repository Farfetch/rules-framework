namespace Rules.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class ConditionNodeProperties
    {
        public static string GetCompiledDelegateKey(string multiplicity)
        {
            if (string.IsNullOrWhiteSpace(multiplicity))
            {
                throw new ArgumentException($"'{nameof(multiplicity)}' cannot be null or whitespace.", nameof(multiplicity));
            }

            return $"_compiled_{multiplicity}";
        }

        public static string CompiledFlagKey => "_compiled";
    }
}
