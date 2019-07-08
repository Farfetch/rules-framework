using Rules.Framework.Core;

namespace Rules.Framework.Serialization
{
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