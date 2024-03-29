namespace Rules.Framework.Providers.InMemory.DataModel
{
    using Rules.Framework.Core;

    internal sealed class ValueConditionNodeDataModel<TConditionType> : ConditionNodeDataModel<TConditionType>
    {
        public TConditionType ConditionType { get; set; }

        public DataTypes DataType { get; set; }

        public object Operand { get; set; }

        public Operators Operator { get; set; }
    }
}