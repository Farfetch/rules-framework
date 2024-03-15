namespace Rules.Framework.Evaluation
{
    using Rules.Framework.Core;

    internal interface IDataTypesConfigurationProvider
    {
        DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType);
    }
}