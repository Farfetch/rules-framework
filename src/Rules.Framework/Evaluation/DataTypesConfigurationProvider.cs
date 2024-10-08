namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;

    internal sealed class DataTypesConfigurationProvider : IDataTypesConfigurationProvider
    {
        private readonly IDictionary<DataTypes, DataTypeConfiguration> dataTypeConfigurations;

        public DataTypesConfigurationProvider(RulesEngineOptions rulesEngineOptions)
        {
            this.dataTypeConfigurations = new Dictionary<DataTypes, DataTypeConfiguration>
            {
                { DataTypes.Integer, DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), rulesEngineOptions.DataTypeDefaults[DataTypes.Integer]) },
                { DataTypes.String, DataTypeConfiguration.Create(DataTypes.String, typeof(string), rulesEngineOptions.DataTypeDefaults[DataTypes.String]) },
                { DataTypes.Decimal, DataTypeConfiguration.Create(DataTypes.Decimal, typeof(decimal), rulesEngineOptions.DataTypeDefaults[DataTypes.Decimal]) },
                { DataTypes.Boolean, DataTypeConfiguration.Create(DataTypes.Boolean, typeof(bool), rulesEngineOptions.DataTypeDefaults[DataTypes.Boolean]) },
                { DataTypes.ArrayInteger, DataTypeConfiguration.Create(DataTypes.ArrayInteger, typeof(int), rulesEngineOptions.DataTypeDefaults[DataTypes.ArrayInteger]) },
                { DataTypes.ArrayString, DataTypeConfiguration.Create(DataTypes.ArrayString, typeof(string), rulesEngineOptions.DataTypeDefaults[DataTypes.ArrayString]) },
                { DataTypes.ArrayDecimal, DataTypeConfiguration.Create(DataTypes.ArrayDecimal, typeof(decimal), rulesEngineOptions.DataTypeDefaults[DataTypes.ArrayDecimal]) },
                { DataTypes.ArrayBoolean, DataTypeConfiguration.Create(DataTypes.ArrayBoolean, typeof(bool), rulesEngineOptions.DataTypeDefaults[DataTypes.ArrayBoolean]) },
            };
        }

        public DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypeConfigurations.TryGetValue(dataType, out DataTypeConfiguration dataTypeConfiguration)
            ? dataTypeConfiguration
            : throw new NotSupportedException($"Data type '{dataType}' is not supported.");
    }
}