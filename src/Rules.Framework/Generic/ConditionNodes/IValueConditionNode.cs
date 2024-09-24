namespace Rules.Framework.Generic.ConditionNodes
{
    using Rules.Framework;
    using Rules.Framework.Generic;

    /// <summary>
    /// Defines the interface contract for a condition node based on a value comparison.
    /// </summary>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public interface IValueConditionNode<TConditionType> : IConditionNode<TConditionType>
    {
        /// <summary>
        /// Gets the condition node type.
        /// </summary>
        TConditionType ConditionType { get; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        DataTypes DataType { get; }

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        object Operand { get; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        Operators Operator { get; }
    }
}