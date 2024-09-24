namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Fluent builder for composed condition nodes.
    /// </summary>
    public interface IFluentComposedConditionNodeBuilder
    {
        /// <summary>
        /// Adds a And composed condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder And(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode Build();

        /// <summary>
        /// Adds a Condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IFluentComposedConditionNodeBuilder Condition(IConditionNode conditionNode);

        /// <summary>
        /// Adds a Or composed condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder Or(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Adds a Value condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder Value<TDataType>(string conditionType, Operators condOperator, TDataType operand);
    }
}