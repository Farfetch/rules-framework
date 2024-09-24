namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal static class GenericConversions
    {
        public static T Convert<T>(string value)
        {
            var valueType = typeof(T);
            if (valueType.IsEnum)
            {
                return (T)Enum.Parse(valueType, value, ignoreCase: false);
            }

            return (T)System.Convert.ChangeType(value, valueType, CultureInfo.InvariantCulture);
        }

        public static string Convert<T>(T value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }

            return value!.ToString();
        }
    }
}