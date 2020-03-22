namespace Rules.Framework.Serialization
{
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    /// <summary>
    /// The rule builder extensions to ease rules creation.
    /// </summary>
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Sets the rule with serialized content.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="serializedContent">Content of the serialized.</param>
        /// <param name="contentSerializationProvider">The content serialization provider.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// ruleBuilder
        /// or
        /// contentSerializationProvider
        /// </exception>
        public static IRuleBuilder<TContentType, TConditionType> WithSerializedContent<TContentType, TConditionType>(
            this IRuleBuilder<TContentType, TConditionType> ruleBuilder,
            TContentType contentType, object serializedContent,
            IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            if (ruleBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(ruleBuilder));
            }

            if (contentSerializationProvider is null)
            {
                throw new System.ArgumentNullException(nameof(contentSerializationProvider));
            }

            ContentContainer<TContentType> contentContainer = new SerializedContentContainer<TContentType>(contentType, serializedContent, contentSerializationProvider);

            return ruleBuilder.WithContentContainer(contentContainer);
        }
    }
}