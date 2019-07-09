namespace Rules.Framework.Serialization
{
    using Rules.Framework.Core;

    public class SerializedContentContainer<TContentType> : ContentContainer<TContentType>
    {
        public SerializedContentContainer(
            TContentType contentType,
            object serializedContent,
            IContentSerializationProvider<TContentType> contentSerializationProvider)
            : base(contentType, (t) => contentSerializationProvider.GetContentSerializer(contentType).Deserialize(serializedContent, t))
        {
        }
    }
}