namespace Rules.Framework.Serialization
{
    /// <summary>
    /// Defines a content container that relies on a <see cref="IContentSerializationProvider"/> to
    /// get a content serializer to deserialize container content.
    /// </summary>
    public class SerializedContentContainer : ContentContainer
    {
        /// <summary>
        /// Creates a new <see cref="SerializedContentContainer"/>.
        /// </summary>
        /// <param name="contentType">the content type.</param>
        /// <param name="serializedContent">the serialized content.</param>
        /// <param name="contentSerializationProvider">the content serialization provider.</param>
        public SerializedContentContainer(
            string contentType,
            object serializedContent,
            IContentSerializationProvider contentSerializationProvider)
            : base((t) => contentSerializationProvider.GetContentSerializer(contentType).Deserialize(serializedContent, t))
        {
        }
    }
}