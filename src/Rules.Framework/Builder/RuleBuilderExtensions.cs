namespace Rules.Framework.Builder
{
    using Rules.Framework.Core;

    /// <summary>
    /// The rule builder extension methods.
    /// </summary>
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> WithContent<TContentType, TConditionType>(
            this IRuleBuilder<TContentType, TConditionType> ruleBuilder,
            TContentType contentType,
            object content)
        {
            ContentContainer<TContentType> contentContainer = new ContentContainer<TContentType>(contentType, (t) => content);

            return ruleBuilder.WithContentContainer(contentContainer);
        }
    }
}