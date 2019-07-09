namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal class RulesEngineOptionsValidator
    {
        public void EnsureValid(RulesEngineOptions rulesEngineOptions)
        {
            if (rulesEngineOptions == null)
            {
                throw new InvalidRulesEngineOptionsException($"Specified null {nameof(rulesEngineOptions)}.");
            }

            this.EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Boolean,
                value => value != null && bool.TryParse(value?.ToString(), out bool boolRes));
            this.EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Decimal,
                value => value != null && decimal.TryParse(value?.ToString(), out decimal decimalRes));
            this.EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.Integer,
                value => value != null && int.TryParse(value?.ToString(), out int intRes));
            this.EnsureValidDataTypeDefault(
                rulesEngineOptions.DataTypeDefaults,
                DataTypes.String,
                value => value != null && value is string);
        }

        private void EnsureValidDataTypeDefault(IDictionary<DataTypes, object> dataTypeDefaults, DataTypes dataType, Func<object, bool> validFunc)
        {
            object value = dataTypeDefaults[dataType];
            if (!validFunc.Invoke(value))
            {
                throw new InvalidRulesEngineOptionsException($"Specified invalid default value for data type {DataTypes.Boolean}: {value}.");
            }
        }
    }
}