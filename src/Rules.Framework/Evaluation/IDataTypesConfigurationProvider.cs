namespace Rules.Framework.Evaluation
{
    using Rules.Framework;

    internal interface IDataTypesConfigurationProvider
    {
        DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType);
    }
}