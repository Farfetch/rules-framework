namespace Rules.Framework.Providers.MongoDb.Serialization
{
    using System;
    using Rules.Framework.Serialization;

    public class DynamicToStrongTypeContentSerializationProvider<TContentType> : IContentSerializationProvider<TContentType>
    {
        private readonly Lazy<IContentSerializer> contentSerializerLazy;

        public DynamicToStrongTypeContentSerializationProvider()
        {
            this.contentSerializerLazy = new Lazy<IContentSerializer>(
                () => new DynamicToStrongTypeContentSerializer(),
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public IContentSerializer GetContentSerializer(TContentType contentType) => this.contentSerializerLazy.Value;
    }
}
