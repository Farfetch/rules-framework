namespace System
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, IDictionary<LanguageOperator, string>> languageOperatorsSupportByType = new();

        public static bool HasLanguageOperator(this Type type, LanguageOperator languageOperator)
        {
            var languageOperatorsByType = languageOperatorsSupportByType.GetOrAdd(type, (t) =>
            {
                return ((TypeInfo)type).GetRuntimeMethods()
                    .Select(m =>
                    {
                        var splittedName = m.Name.Split('.');
                        return splittedName[splittedName.Length - 1];
                    })
                    .Distinct(StringComparer.Ordinal)
                    .Select(m => new { LanguageOperator = m.ToLanguageOperator(), Name = m })
                    .Where(x => x.LanguageOperator != LanguageOperator.None)
                    .ToDictionary(x => x.LanguageOperator, x => x.Name);
            });

            return languageOperatorsByType.ContainsKey(languageOperator);
        }

        private static LanguageOperator ToLanguageOperator(this string operatorMethodName) => operatorMethodName switch
        {
            "op_Equality" => LanguageOperator.Equal,
            "op_Inequality" => LanguageOperator.NotEqual,
            "op_GreaterThan" => LanguageOperator.GreaterThan,
            "op_GreaterThanOrEqual" => LanguageOperator.GreaterThanOrEqual,
            "op_LessThan" => LanguageOperator.LessThan,
            "op_LessThanOrEqual" => LanguageOperator.LessThanOrEqual,
            _ => LanguageOperator.None,
        };
    }
}