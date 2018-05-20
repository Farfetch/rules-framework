using System;

namespace Rules.Framework.Serialization
{
    public interface IContentSerializer
    {
        object Deserialize(object serializedContent, Type type);

        object Serialize(object content);
    }
}