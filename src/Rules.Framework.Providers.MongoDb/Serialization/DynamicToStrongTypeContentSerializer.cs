namespace Rules.Framework.Providers.MongoDb.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Rules.Framework.Serialization;

    internal class DynamicToStrongTypeContentSerializer : IContentSerializer
    {
        public object Deserialize(object serializedContent, Type type)
        {
            if (serializedContent is null)
            {
                throw new ArgumentNullException(nameof(serializedContent));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!(serializedContent is ExpandoObject))
            {
                throw new NotSupportedException($"The serialized content type is not supported for deserialization: {type.FullName}");
            }

            IDictionary<string, object> serializedContentDictionary = serializedContent as IDictionary<string, object>;
            IDictionary<string, PropertyInfo> reflectionInformation = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(x => x.Name);
            object deserializedRepresentation = Activator.CreateInstance(type);


            foreach (string key in serializedContentDictionary.Keys)
            {
                if (reflectionInformation.TryGetValue(key, out PropertyInfo currentPropertyInfo))
                {
                    currentPropertyInfo.SetValue(deserializedRepresentation, Parse(serializedContentDictionary[key], currentPropertyInfo.PropertyType));
                }
                else
                {
                    throw new SerializationException($"Property '{key}' does not have a matching property by the same name on type '{type.FullName}'.");
                }
            }

            return deserializedRepresentation;
        }

        private static object Parse(object value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else if (type.IsAssignableFrom(typeof(Guid)))
            {
                return Guid.Parse(value.ToString());
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        public object Serialize(object content) => throw new NotImplementedException();
    }
}
