using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Rules.Framework.Builder.Validation
{
    internal static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> IsContainedOn<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilderInitial, IEnumerable<TProperty> values)
        {
            return ruleBuilderInitial.Must((p) => values.Contains(p));
        }

        public static IRuleBuilderOptions<T, TProperty> IsContainedOn<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilderInitial, params TProperty[] values)
        {
            return ruleBuilderInitial.IsContainedOn((IEnumerable<TProperty>)values);
        }
    }
}