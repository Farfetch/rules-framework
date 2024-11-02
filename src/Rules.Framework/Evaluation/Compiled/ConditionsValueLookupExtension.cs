namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal static class ConditionsValueLookupExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValueOrDefault(IDictionary<string, object> conditions, string conditionName)
        {
            if (conditions.TryGetValue(conditionName, out var conditionValue))
            {
                return conditionValue;
            }

            return null!;
        }
    }
}