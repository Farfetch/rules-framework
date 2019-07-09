namespace Rules.Framework.Serialization
{
    using System;

    public interface IContentSerializer
    {
        object Deserialize(object serializedContent, Type type);

        object Serialize(object content);
    }
}