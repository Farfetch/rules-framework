namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using Rules.Framework;

    /// <summary>
    /// Builder for condition nodes.
    /// </summary>
    public interface IFluentConditionNodeBuilder
    {
        /// <summary>
        /// Adds a 'and' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder And(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Builds the composed condition node.
        /// </summary>
        /// <returns></returns>
        IConditionNode Build();

        /// <summary>
        /// Adds a pre-built <see cref="IConditionNode"/> to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionNode">The condition node.</param>
        IFluentConditionNodeBuilder Condition(IConditionNode conditionNode);

        /// <summary>
        /// Adds a 'or' composed condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="conditionFunc">The function containing the logic for the new condition.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder Or(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc);

        /// <summary>
        /// Adds a 'value' condition to the new rule's conditions tree.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        IFluentConditionNodeBuilder Value<T>(string condition, Operators condOperator, T operand);
    }
}