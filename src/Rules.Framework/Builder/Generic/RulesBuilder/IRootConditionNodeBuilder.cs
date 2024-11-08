namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Builder for the root condition node.
    /// </summary>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRootConditionNodeBuilder<TCondition>
    {
        /// <summary>
        /// Sets a And composed condition to the root condition node builder.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode And(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc);

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
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc);

        /// <summary>
        /// Sets a Value condition to the root condition node builder.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IConditionNode Value<T>(TCondition condition, Operators condOperator, T operand);
    }
}