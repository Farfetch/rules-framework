namespace Rules.Framework.Serialization
{
    /// <summary>
    /// Defines the interface contract for a content serialization provider. Provides content serializers per
    /// <typeparamref name="TContentType"/> value, allowing for customization of serializers per each content type.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    public interface IContentSerializationProvider<in TContentType>
    {
        /// <summary>
        /// Gets the content serializer associated with the given <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contentType">the content type.</param>
        /// <returns>the content serializer to deal with contents for specified content type.</returns>
        IContentSerializer GetContentSerializer(TContentType contentType);
    }
}