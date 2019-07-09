namespace Rules.Framework.IntegrationTests
{
    using System;
    using Newtonsoft.Json;
    using Rules.Framework.Serialization;

    internal class JsonContentSerializer : IContentSerializer
    {
        public object Deserialize(object serializedContent, Type type)
        {
            return JsonConvert.DeserializeObject(serializedContent.ToString(), type);
        }

        public object Serialize(object content)
        {
            return JsonConvert.SerializeObject(content);
        }
    }
}