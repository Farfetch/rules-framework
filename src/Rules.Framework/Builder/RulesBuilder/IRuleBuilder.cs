namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using Rules.Framework;
    using Rules.Framework.Builder;

    /// <summary>
    /// Builder to create a new rule.
    /// </summary>
    public interface IRuleBuilder
    {
        /// <summary>
        /// Sets the new rule to apply when the specified condition matches.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        IRuleBuilder ApplyWhen(IConditionNode condition);

        /// <summary>
        /// Sets the new rule to apply when the condition specified by the given parameters matches.
        /// </summary>
        /// <typeparam name="T">The type of the operands.</typeparam>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        IRuleBuilder ApplyWhen<T>(string condition, Operators condOperator, T operand);

        /// <summary>
        /// Sets the new rule to apply when the condition given by the specified builder matches.
        /// </summary>
        /// <param name="conditionFunc">The condition func.</param>
        /// <returns></returns>
        IRuleBuilder ApplyWhen(Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc);

        /// <summary>
        /// Builds the new rule.
        /// </summary>
        /// <returns></returns>
        RuleBuilderResult Build();

        /// <summary>
        /// Sets the new rule with the specified active status.
        /// </summary>
        /// <param name="active">The active status.</param>
        /// <returns></returns>
        IRuleBuilder WithActive(bool active);
    }
}