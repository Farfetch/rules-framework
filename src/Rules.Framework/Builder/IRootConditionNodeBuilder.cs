namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Builder for the root condition node.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IRootConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Sets a And composed condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode<TConditionType> And(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Sets a Condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IConditionNode<TConditionType> Condition(IConditionNode<TConditionType> conditionNode);

        /// <summary>
        /// Sets a Or composed condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode<TConditionType> Or(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Sets a Value condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IConditionNode<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);
    }
}