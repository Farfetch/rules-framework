namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Builder for condition nodes.
    /// </summary>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IFluentConditionNodeBuilder<TCondition>
    {
        /// <summary>
        /// Adds a 'and' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TCondition> And(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode Build();

        /// <summary>
        /// Adds a pre-built <see cref="IConditionNode"/> to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IFluentConditionNodeBuilder<TCondition> Condition(IConditionNode conditionNode);

        /// <summary>
        /// Adds a 'or' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TCondition> Or(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc);

        /// <summary>
        /// Adds a 'value' condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder<TCondition> Value<T>(TCondition condition, Operators condOperator, T operand);
    }
}