namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.Core;

    internal abstract class ConditionEvalDispatcherBase
    {
        private readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;

        protected ConditionEvalDispatcherBase(IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
        }

        protected static object ConvertToDataType(object value, DataTypeConfiguration dataTypeConfiguration)
            => Convert.ChangeType(value
                ?? dataTypeConfiguration.Default, dataTypeConfiguration.Type, CultureInfo.InvariantCulture);

        protected static IEnumerable<object> ConvertToTypedEnumerable(object rightOperand, string paramName)
        {
            if (rightOperand is IEnumerable enumerable)
            {
                return enumerable.Cast<object>();
            }

            throw new ArgumentException($"Parameter must be of type {nameof(IEnumerable)}.", paramName);
        }

        protected DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);
    }
}