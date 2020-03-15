namespace Rules.Framework.Serialization
{
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    public static class RuleBuilderExtensions
    {
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
