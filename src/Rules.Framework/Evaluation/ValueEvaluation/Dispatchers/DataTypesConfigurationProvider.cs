namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal class DataTypesConfigurationProvider : IDataTypesConfigurationProvider
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
            };
        }

        public DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypeConfigurations.TryGetValue(dataType, out DataTypeConfiguration dataTypeConfiguration)
            ? dataTypeConfiguration
            : throw new NotSupportedException($"Data type '{dataType}' is not supported.");
    }
}