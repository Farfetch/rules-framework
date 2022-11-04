namespace Rules.Framework.Generic
{
    using Rules.Framework.Core;

    public class GenericValueConditionNode<ConditionType> : GenericConditionNode<ConditionType>
    {
        /// <summary>
        /// Gets the condition node type name.
        /// </summary>
        public string ConditionTypeName { get; internal set; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public DataTypes DataType { get; internal set; }

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        public object Operand { get; internal set; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        public Operators Operator { get; internal set; }
    }
}