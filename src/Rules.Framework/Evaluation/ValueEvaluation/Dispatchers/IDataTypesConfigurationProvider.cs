namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using Rules.Framework.Core;

    internal interface IDataTypesConfigurationProvider
    {
        DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType);
    }
}