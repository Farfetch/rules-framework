namespace Rules.Framework.Providers.SqlServer.DataModel
{
    using Rules.Framework.Core;

    internal class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        public string ConditionType { get; set; }

        public DataTypes DataType { get; set; }

        public object Operand { get; set; }
        public Operators Operator { get; set; }
    }
}