namespace Rules.Framework.Builder
{
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    /// <summary>
    /// Builder to specify the data type for a valued condition node.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IValueConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Sets the new value condition node to have the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">the data type of the new value condition node.</typeparam>
        /// <returns></returns>
        IValueConditionNodeBuilder<TConditionType, T> OfDataType<T>();
    }

    /// <summary>
    /// Builder to specify create a new value condition node.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    /// <typeparam name="TDataType">The type of the data type.</typeparam>
    public interface IValueConditionNodeBuilder<TConditionType, TDataType>
    {
        /// <summary>
        /// Builds the new value condition node.
        /// </summary>
        /// <returns></returns>
        IValueConditionNode<TConditionType> Build();

        /// <summary>
        /// Sets the condition node right hand operand (as a single value). Remember that the rules
        /// engine input conditions will be evaluated as left hand operands.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(TDataType value);

        /// <summary>
        /// Sets the condition node right hand operand (as collection of values). Remember that the
        /// rules engine input conditions will be evaluated as left hand operands.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(IEnumerable<TDataType> value);

        /// <summary>
        /// Sets the new value condition node with the specified comparison operator.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <returns></returns>
        IValueConditionNodeBuilder<TConditionType, TDataType> WithComparisonOperator(Operators comparisonOperator);

        IValueConditionNodeBuilder<TConditionType, TDataType> WithInternalId(object internalId);
    }
}