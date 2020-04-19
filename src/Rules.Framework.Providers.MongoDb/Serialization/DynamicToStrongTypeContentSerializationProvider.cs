namespace Rules.Framework.Providers.MongoDb.Serialization
{
    using System;
    using Rules.Framework.Serialization;

    /// <summary>
    /// Defines a content serialization provider for dynamic types.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <seealso cref="Rules.Framework.Serialization.IContentSerializationProvider{TContentType}" />
    public class DynamicToStrongTypeContentSerializationProvider<TContentType> : IContentSerializationProvider<TContentType>
    {
        private readonly Lazy<IContentSerializer> contentSerializerLazy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicToStrongTypeContentSerializationProvider{TContentType}"/> class.
        /// </summary>
        public DynamicToStrongTypeContentSerializationProvider()
        {
            this.contentSerializerLazy = new Lazy<IContentSerializer>(
                () => new DynamicToStrongTypeContentSerializer(),
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Gets the content serializer associated with the given <paramref name="contentType" />.
        /// </summary>
        /// <param name="contentType">the content type.</param>
        /// <returns>
        /// the content serializer to deal with contents for specified content type.
        /// </returns>
        public IContentSerializer GetContentSerializer(TContentType contentType) => this.contentSerializerLazy.Value;
    }
}