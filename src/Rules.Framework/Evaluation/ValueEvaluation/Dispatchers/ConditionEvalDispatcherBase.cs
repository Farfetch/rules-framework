namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using System.Globalization;
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

        protected DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
                    => this.dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);
    }
}