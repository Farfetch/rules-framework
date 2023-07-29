namespace Rules.Framework.Providers.MongoDb.Serialization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Rules.Framework.Serialization;

    internal sealed class DynamicToStrongTypeContentSerializer : IContentSerializer
    {
        private static readonly Type expandoObjectType = typeof(ExpandoObject);

        private static readonly Type objectType = typeof(object);

        private static readonly Type stringType = typeof(string);

        private static readonly ConcurrentDictionary<string, IDictionary<string, PropertyInfo>> typePropertiesCache
                            = new ConcurrentDictionary<string, IDictionary<string, PropertyInfo>>(StringComparer.Ordinal);

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

            var serializedContentType = serializedContent.GetType();
            if (serializedContentType.IsValueType || stringType.IsAssignableFrom(serializedContentType))
            {
                return Parse(serializedContent, type);
            }

            if (!expandoObjectType.IsAssignableFrom(serializedContentType))
            {
                throw new NotSupportedException($"The serialized content type is not supported for deserialization: {serializedContent.GetType().FullName}");
            }

            if (type == objectType || type == expandoObjectType)
            {
                return serializedContent;
            }

            return DeserializeToType((IDictionary<string, object>)serializedContent, type);
        }

        public object Serialize(object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var type = content.GetType();

            if (type.IsEnum)
            {
                // Returns the enum literal name, and if that's not available (i.e. no literal
                // defined for the value), return the value itself
                return Enum.GetName(type, content) ?? content;
            }

            if (type == stringType || type == expandoObjectType)
            {
                return content;
            }

            var reflectedProperties = GetReflectedProperties(type);
            var serializedContent = new ExpandoObject();
            foreach (var key in reflectedProperties.Keys)
            {
                var propertyValue = reflectedProperties[key].GetValue(content);
                serializedContent.TryAdd(key, propertyValue);
            }

            return serializedContent;
        }

        private static object DeserializeToType(IDictionary<string, object> serializedContent, Type type)
        {
            var serializedContentDictionary = serializedContent;
            var reflectedProperties = GetReflectedProperties(type);
            object deserializedRepresentation;
            try
            {
                deserializedRepresentation = Activator.CreateInstance(type, nonPublic: true);
            }
            catch (MissingMethodException mme) when (mme.Message.Contains("parameterless", StringComparison.Ordinal))
            {
                throw new NotSupportedException($"The target type '{type.FullName}' must define a default (no parameters) constructor.", mme);
            }

            foreach (string key in serializedContentDictionary.Keys)
            {
                if (reflectedProperties.TryGetValue(key, out PropertyInfo currentPropertyInfo))
                {
                    var serializedPropertyValue = serializedContentDictionary[key];

                    try
                    {
                        currentPropertyInfo.SetValue(deserializedRepresentation, Parse(serializedPropertyValue, currentPropertyInfo.PropertyType));
                    }
                    catch (FormatException fe)
                    {
                        throw new SerializationException($"An invalid value has been provided for property '{key}' of type '{type.FullName}': '{serializedPropertyValue}'.", fe);
                    }
                }
                else
                {
                    throw new SerializationException($"Property '{key}' does not have a matching property by the same name on type '{type.FullName}'.");
                }
            }

            return deserializedRepresentation;
        }

        private static IDictionary<string, PropertyInfo> GetReflectedProperties(Type type) => typePropertiesCache.GetOrAdd(
            type.FullName,
            (_, t) => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name, StringComparer.Ordinal),
            type);

        private static object Parse(object value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }

            if (typeof(Guid).IsAssignableFrom(type))
            {
                return Guid.Parse(value.ToString());
            }

            // InvariantCulture for all formats is an assumption.
            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
    }
}