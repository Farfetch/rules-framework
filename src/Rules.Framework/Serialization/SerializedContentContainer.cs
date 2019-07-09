namespace Rules.Framework.Serialization
{
    using Rules.Framework.Core;

    /// <summary>
    /// Defines a content container that relies on a <see cref="IContentSerializationProvider{TContentType}"/> to get a content serializer to deserialize container content.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    public class SerializedContentContainer<TContentType> : ContentContainer<TContentType>
    {
        /// <summary>
        /// Creates a new <see cref="SerializedContentContainer{TContentType}"/>.
        /// </summary>
        /// <param name="contentType">the content type.</param>
        /// <param name="serializedContent">the serialized content.</param>
        /// <param name="contentSerializationProvider">the content serialization provider.</param>
        public SerializedContentContainer(
            TContentType contentType,
            object serializedContent,
            IContentSerializationProvider<TContentType> contentSerializationProvider)
            : base(contentType, (t) => contentSerializationProvider.GetContentSerializer(contentType).Deserialize(serializedContent, t))
        {
        }
    }
}