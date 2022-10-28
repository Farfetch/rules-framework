namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal static class RulesEngineOptionsValidator
    {
        public static void EnsureValid(RulesEngineOptions rulesEngineOptions)
        {
            if (rulesEngineOptions == null)
            {
                throw new InvalidRulesEngineOptionsException($"Specified null {nameof(rulesEngineOptions)}.");
            }

            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Boolean,
                value => value != null && bool.TryParse(value.ToString(), out bool boolRes));
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Decimal,
                value => value != null && decimal.TryParse(value.ToString(), out decimal decimalRes));
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Integer,
                value => value != null && int.TryParse(value.ToString(), out int intRes));
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.String,
                value => value is string);
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayBoolean,
                value => value is IEnumerable<bool>);
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayDecimal,
                value => value is IEnumerable<decimal>);
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayInteger,
                value => value is IEnumerable<int>);
            EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.ArrayString,
                value => value is IEnumerable<string>);

        }

        private static void EnsureValidDataTypeDefault(IDictionary<DataTypes, object> dataTypeDefaults, DataTypes dataType, Func<object, bool> validFunc)
        {
            object value = dataTypeDefaults[dataType];
            if (!validFunc.Invoke(value))
            {
                throw new InvalidRulesEngineOptionsException($"Specified invalid default value for data type {dataType}: {value ?? "null"}.");
            }
        }
    }
}