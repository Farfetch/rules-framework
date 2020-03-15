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
        /// Adds a condition to the composed condition node builder.
        /// </summary>
        /// <param name="addConditionFunc">The function containing the logic for new to add condition.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> addConditionFunc);

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