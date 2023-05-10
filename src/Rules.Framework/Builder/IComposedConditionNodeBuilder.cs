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
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        [Obsolete("This way of adding conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a composed and condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        public IComposedConditionNodeBuilder<TConditionType> And(
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode<TConditionType> Build();

        /// <summary>
        /// Adds a composed or condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        public IComposedConditionNodeBuilder<TConditionType> Or(
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a value condition to the composed condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);

        /// <summary>
        /// Sets the composed condition node with the specified logical operator.
        /// </summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns></returns>
        //[Obsolete("This way of composing conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator);
    }
}