namespace Rules.Framework.IntegrationTests
{
    using Rules.Framework.Serialization;

    internal class JsonContentSerializationProvider : IContentSerializationProvider
    {
        public IContentSerializer GetContentSerializer(string contentType)
        {
            return new JsonContentSerializer();
        }
    }
}