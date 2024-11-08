namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Builder for the root condition node.
    /// </summary>
    public interface IRootConditionNodeBuilder
    {
        /// <summary>
        /// Adds a 'and' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode And(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Adds a pre-built <see cref="IConditionNode"/> to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IConditionNode Condition(IConditionNode conditionNode);

        /// <summary>
        /// Adds a 'or' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the root condition.</param>
        /// <returns></returns>
        IConditionNode Or(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Adds a 'value' condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IConditionNode Value<T>(string condition, Operators condOperator, T operand);
    }
}