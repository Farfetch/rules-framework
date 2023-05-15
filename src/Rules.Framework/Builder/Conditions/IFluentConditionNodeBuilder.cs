namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Fluent builder for composed condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IFluentConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Adds a composed And condition to the child condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TConditionType> And(
            Func<IFluentConditionNodeBuilder<TConditionType>, IFluentConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Builds the child condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode<TConditionType> Build();

        /// <summary>
        /// Adds a composed Or condition to the child condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TConditionType> Or(
            Func<IFluentConditionNodeBuilder<TConditionType>, IFluentConditionNodeBuilder<TConditionType>> conditionFunc);

        /// <summary>
        /// Adds a value condition to the child condition node builder.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);
    }
}