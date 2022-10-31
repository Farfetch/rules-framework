namespace Rules.Framework.Generic
{
    using Rules.Framework.Core;

    public class GenericValueConditionNode<ConditionType> : GenericConditionNode<ConditionType>
    {
        public string ConditionTypeName { get; set; }

        public DataTypes DataType { get; set; }

        public object Operand { get; set; }

        public Operators Operator { get; set; }
    }
}