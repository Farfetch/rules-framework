namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Rules.Framework;

    internal static class RulesEngineOptionsValidator
    {
        public static void Validate(RulesEngineOptions rulesEngineOptions)
        {
            if (rulesEngineOptions == null)
            {
                throw new InvalidRulesEngineOptionsException($"Specified null {nameof(rulesEngineOptions)}.");
            }

            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Boolean,
                value => value != null && bool.TryParse(value.ToString(), out bool _));
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Decimal,
                value => value != null && decimal.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out decimal _));
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Integer,
                value => value != null && int.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out int _));
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.String,
                value => value is string);
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayBoolean,
                value => value is bool);
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayDecimal,
                value => value is decimal);
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayInteger,
                value => value is int);
            ValidateDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayString,
                value => value is string);

        }

        private static void ValidateDataTypeDefault(IDictionary<DataTypes, object> dataTypeDefaults, DataTypes dataType, Func<object, bool> validFunc)
        {
            object value = dataTypeDefaults[dataType];
            if (!validFunc.Invoke(value))
            {
                throw new InvalidRulesEngineOptionsException($"Specified invalid default value for data type {dataType}: {value ?? "null"}.");
            }
        }
    }
}