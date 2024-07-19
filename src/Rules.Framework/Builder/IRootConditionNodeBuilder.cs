namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Builder for the root condition node.
    /// </summary>
    public interface IRootConditionNodeBuilder
    {
        /// <summary>
        /// Sets a And composed condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode And(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Sets a Condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IConditionNode Condition(IConditionNode conditionNode);

        /// <summary>
        /// Sets a Or composed condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode Or(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Sets a Value condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IConditionNode Value<TDataType>(string conditionType, Operators condOperator, TDataType operand);
    }
}