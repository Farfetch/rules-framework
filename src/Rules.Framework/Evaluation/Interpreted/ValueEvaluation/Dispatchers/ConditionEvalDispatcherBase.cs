namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
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

        protected static object ConvertToDataType(object operand, string paramName, DataTypeConfiguration dataTypeConfiguration)
        {
            try
            {
                return Convert.ChangeType(operand
                    ?? dataTypeConfiguration.Default, dataTypeConfiguration.Type, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException ice)
            {
                throw new ArgumentException($"Parameter value or contained value is not convertible to {dataTypeConfiguration.Type.Name}.", paramName, ice);
            }
        }

        protected static IEnumerable<object> ConvertToTypedEnumerable(object operand, string paramName)
        {
            if (operand is IEnumerable enumerable)
            {
                return enumerable.Cast<object>();
            }

            throw new ArgumentException($"Parameter must be of type {nameof(IEnumerable)}.", paramName);
        }

        protected DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);
    }
}