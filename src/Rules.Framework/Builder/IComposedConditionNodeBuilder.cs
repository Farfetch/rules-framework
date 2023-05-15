namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Builder for composed condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    [Obsolete("This way of building and adding composed conditions has been deprecated.")]
    public interface IComposedConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Adds a condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        [Obsolete("This way of adding conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc);

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
        [Obsolete("This way of composing conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator);
    }
}