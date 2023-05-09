namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Builder for composed condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IComposedConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Adds a composed condition to the composed condition node builder.
        /// </summary>
        /// <param name="logicOperator">The logical operator.</param>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> AddComposedCondition(
            LogicalOperators logicOperator,
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a value condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> AddValueCondition<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode<TConditionType> Build();

        /// <summary>
        /// Sets the composed condition node with the specified logical operator.
        /// </summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator);
    }
}