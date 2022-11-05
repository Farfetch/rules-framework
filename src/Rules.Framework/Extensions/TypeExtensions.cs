namespace System
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, IDictionary<LanguageOperator, bool>> languageOperatorsSupportByType
            = new ConcurrentDictionary<Type, IDictionary<LanguageOperator, bool>>();

        public static bool HasLanguageOperator(this Type type, LanguageOperator languageOperator)
        {
            var languageOperatorsByType = languageOperatorsSupportByType.GetOrAdd(type, (t) => new Dictionary<LanguageOperator, bool>()
            {
                { LanguageOperator.Equal, t.HasLanguageOperatorInternal(LanguageOperator.Equal) },
                { LanguageOperator.NotEqual, t.HasLanguageOperatorInternal(LanguageOperator.NotEqual) },
                { LanguageOperator.GreaterThan, t.HasLanguageOperatorInternal(LanguageOperator.GreaterThan) },
                { LanguageOperator.GreaterThanOrEqual, t.HasLanguageOperatorInternal(LanguageOperator.GreaterThanOrEqual) },
                { LanguageOperator.LessThan, t.HasLanguageOperatorInternal(LanguageOperator.LessThan) },
                { LanguageOperator.LessThanOrEqual, t.HasLanguageOperatorInternal(LanguageOperator.LessThanOrEqual) },
            });

            if (languageOperatorsByType.TryGetValue(languageOperator, out bool result))
            {
                return result;
            }

            return false;
        }

        private static bool HasLanguageOperatorInternal(this Type type, LanguageOperator languageOperator)
            => ((TypeInfo)type).GetRuntimeMethods().Any(m => m.Name.Contains(languageOperator.ToOperatorMethodName()));

        private static string ToOperatorMethodName(this LanguageOperator languageOperator) => languageOperator switch
        {
            LanguageOperator.Equal => "op_Equality",
            LanguageOperator.NotEqual => "op_Inequality",
            LanguageOperator.GreaterThan => "op_GreaterThan",
            LanguageOperator.GreaterThanOrEqual => "op_GreaterThanOrEqual",
            LanguageOperator.LessThan => "op_LessThan",
            LanguageOperator.LessThanOrEqual => "op_LessThanOrEqual",
            _ => throw new NotSupportedException()
        };
    }
}
