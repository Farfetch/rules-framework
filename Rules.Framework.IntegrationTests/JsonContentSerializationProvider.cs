using Rules.Framework.Serialization;

namespace Rules.Framework.IntegrationTests
{
    internal class JsonContentSerializationProvider<TContentType> : IContentSerializationProvider<TContentType>
    {
        public IContentSerializer GetContentSerializer(TContentType contentType)
        {
            return new JsonContentSerializer();
        }
    }
}