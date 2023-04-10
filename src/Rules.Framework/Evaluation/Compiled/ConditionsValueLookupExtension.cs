namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;

    internal static class ConditionsValueLookupExtension
    {
        public static object GetValueOrDefault<TConditionType>(IDictionary<TConditionType, object> conditions, TConditionType conditionType)
        {
            if (conditions.TryGetValue(conditionType, out var conditionValue))
            {
                return conditionValue;
            }

            return null;
        }
    }
}