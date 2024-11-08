namespace Rules.Framework.Builder.Generic
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Generic;

    /// <summary>
    /// Contains the results information from a generic rule build operation.
    /// </summary>
    public class RuleBuilderResult<TContentType, TConditionType> : RuleBuilderResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBuilderResult"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="rule">The rule.</param>
        /// <param name="errors">The errors.</param>
        internal RuleBuilderResult(bool isSuccess, Rule<TContentType, TConditionType> rule, IEnumerable<string> errors)
            : base(isSuccess, errors)
        {
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public Rule<TContentType, TConditionType> Rule { get; }

        /// <summary>
        /// Creates a result marked with failure.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">errors</exception>
        public static RuleBuilderResult<TContentType, TConditionType> Failure(IEnumerable<string> errors)
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
        public static RuleBuilderResult<TContentType, TConditionType> Success(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new System.ArgumentNullException(nameof(rule));
            }

            return new RuleBuilderResult<TContentType, TConditionType>(isSuccess: true, rule, Enumerable.Empty<string>());
        }
    }
}