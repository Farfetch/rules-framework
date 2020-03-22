namespace Rules.Framework
{
    using Rules.Framework.Builder;

    /// <summary>
    /// The builder to create a new rule.
    /// </summary>
    public static class RuleBuilder
    {
        /// <summary>
        /// Creates a new rule.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> NewRule<TContentType, TConditionType>()
            => new RuleBuilder<TContentType, TConditionType>();
    }
}