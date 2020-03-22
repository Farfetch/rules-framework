namespace Rules.Framework.Providers.MongoDb.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
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
                throw new NotSupportedException($"The serialized content type is not supported for deserialization: {serializedContent.GetType().FullName}");
            }

            IDictionary<string, object> serializedContentDictionary = serializedContent as IDictionary<string, object>;
            IDictionary<string, PropertyInfo> reflectionInformation = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(x => x.Name);
            object deserializedRepresentation = null;

            try
            {
                deserializedRepresentation = Activator.CreateInstance(type, true);
            }
            catch (MissingMethodException mme) when (mme.Message.Contains("parameterless"))
            {
                throw new NotSupportedException($"The target type '{type.FullName}' must define a default (no parameters) constructor.", mme);
            }

            foreach (string key in serializedContentDictionary.Keys)
            {
                if (reflectionInformation.TryGetValue(key, out PropertyInfo currentPropertyInfo))
                {
                    object serializedPropertyValue = serializedContentDictionary[key];

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
                // InvariantCulture for all formats is an assumption.
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
        }

        public object Serialize(object content) => throw new NotImplementedException();
    }
}