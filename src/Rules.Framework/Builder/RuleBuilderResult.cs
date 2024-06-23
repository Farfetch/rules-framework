namespace Rules.Framework.Builder
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    /// <summary>
    /// Allows creation of a new <see cref="RuleBuilderResult{TContentType, TConditionType}"/>.
    /// </summary>
    public static class RuleBuilderResult
    {
        /// <summary>
        /// Creates a result marked with failure.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">errors</exception>
        public static RuleBuilderResult<TContentType, TConditionType> Failure<TContentType, TConditionType>(IEnumerable<string> errors)
        {
            if (errors is null)
            {
                throw new System.ArgumentNullException(nameof(errors));
            }

            return new RuleBuilderResult<TContentType, TConditionType>(isSuccess: false, null!, errors);
        }

        /// <summary>
        /// Creates a result marked with success.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rule</exception>
        public static RuleBuilderResult<TContentType, TConditionType> Success<TContentType, TConditionType>(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new System.ArgumentNullException(nameof(rule));
            }

            return new RuleBuilderResult<TContentType, TConditionType>(isSuccess: true, rule, Enumerable.Empty<string>());
        }
    }

    /// <summary>
    /// Contains the results information from a rule build operation.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public class RuleBuilderResult<TContentType, TConditionType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBuilderResult{TContentType,
        /// TConditionType}"/> class.
        /// </summary>
        internal RuleBuilderResult(bool isSuccess, Rule<TContentType, TConditionType> rule, IEnumerable<string> errors)
        {
            this.IsSuccess = isSuccess;
            this.Rule = rule;
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether rule was built successfuly without validation errors.
        /// </summary>
        /// <value><c>true</c> if rule was built; otherwise, <c>false</c>.</value>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public Rule<TContentType, TConditionType> Rule { get; }
    }
}