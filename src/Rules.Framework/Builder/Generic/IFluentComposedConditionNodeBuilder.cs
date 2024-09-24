namespace Rules.Framework.Builder.Generic
{
    using System;
    using Rules.Framework;
    using Rules.Framework.Generic;

    /// <summary>
    /// Fluent builder for composed condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IFluentComposedConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Adds a And composed condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder<TConditionType> And(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode Build();

        /// <summary>
        /// Adds a Condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IFluentComposedConditionNodeBuilder<TConditionType> Condition(IConditionNode conditionNode);

        /// <summary>
        /// Adds a Or composed condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder<TConditionType> Or(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a Value condition to the fluent condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IFluentComposedConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);
    }
}