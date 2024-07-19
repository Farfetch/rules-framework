namespace Rules.Framework.ConditionNodes
{
    using Rules.Framework;

    /// <summary>
    /// Defines the interface contract for a condition node based on a value comparison.
    /// </summary>
    public interface IValueConditionNode : IConditionNode
    {
        /// <summary>
        /// Gets the condition node type.
        /// </summary>
        string ConditionType { get; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        DataTypes DataType { get; }

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        public object Operand { get; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        Operators Operator { get; }
    }
}