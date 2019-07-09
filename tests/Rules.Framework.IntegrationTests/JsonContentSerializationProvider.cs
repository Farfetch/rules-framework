namespace Rules.Framework.IntegrationTests
{
    using Rules.Framework.Serialization;

    internal class JsonContentSerializationProvider<TContentType> : IContentSerializationProvider<TContentType>
    {
        public IContentSerializer GetContentSerializer(TContentType contentType)
        {
            return new JsonContentSerializer();
        }
    }
}