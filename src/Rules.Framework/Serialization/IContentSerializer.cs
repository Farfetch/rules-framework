namespace Rules.Framework.Serialization
{
    using System;

    /// <summary>
    /// Defines the interface contract for a content serializer. Exposes functionality to serialize and deserialize content.
    /// </summary>
    public interface IContentSerializer
    {
        /// <summary>
        /// Deserializes the provided <paramref name="serializedContent"/> to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="serializedContent">the serialized content.</param>
        /// <param name="type">the type to deserialize to.</param>
        /// <returns>the deserialized representation of the content.</returns>
        object Deserialize(object serializedContent, Type type);

        /// <summary>
        /// Serializes the content.
        /// </summary>
        /// <param name="content">the content.</param>
        /// <returns></returns>
        object Serialize(object content);
    }
}