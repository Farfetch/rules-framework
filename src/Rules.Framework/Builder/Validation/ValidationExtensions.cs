namespace Rules.Framework.Builder.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;

    internal static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> IsContainedOn<T, TProperty>(
            this FluentValidation.IRuleBuilder<T, TProperty> ruleBuilder, IEnumerable<TProperty> values)
        {
            return ruleBuilder.Must((p) => values.Contains(p));
        }

        public static IRuleBuilderOptions<T, TProperty> IsContainedOn<T, TProperty>(
            this FluentValidation.IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] values)
        {
            return ruleBuilder.IsContainedOn((IEnumerable<TProperty>)values);
        }
    }
}