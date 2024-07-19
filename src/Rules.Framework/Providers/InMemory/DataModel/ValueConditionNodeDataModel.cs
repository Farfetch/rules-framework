namespace Rules.Framework.Providers.InMemory.DataModel
{
    using Rules.Framework;

    internal sealed class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        public string Condition { get; set; }

        public DataTypes DataType { get; set; }

        public object Operand { get; set; }

        public Operators Operator { get; set; }
    }
}