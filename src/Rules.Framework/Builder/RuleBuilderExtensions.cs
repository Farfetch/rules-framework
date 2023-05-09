namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// The rule builder extension methods.
    /// </summary>
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Sets the new rule with the specified composed condition.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="logicOperator">The operator.</param>
        /// <param name="conditionFunc">The condition func.</param>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> WithComposedCondition<TContentType, TConditionType>(
            this IRuleBuilder<TContentType, TConditionType> ruleBuilder,
            LogicalOperators logicOperator,
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNodeBuilder = new ConditionNodeBuilder<TConditionType>()
                .AsComposed()
                .WithLogicalOperator(logicOperator);

            var composedConditionBuilder = conditionFunc.Invoke(composedConditionNodeBuilder);

            return ruleBuilder.WithCondition(composedConditionBuilder.Build());
        }

        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> WithContent<TContentType, TConditionType>(
            this IRuleBuilder<TContentType, TConditionType> ruleBuilder,
            TContentType contentType,
            object content)
        {
            var contentContainer = new ContentContainer<TContentType>(contentType, (t) => content);

            return ruleBuilder.WithContentContainer(contentContainer);
        }

        /// <summary>
        /// Sets the new rule with the specified value condition.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <typeparam name="TDataType">The type of the data type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="conditionType">The content type.</param>
        /// <param name="condOperator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> WithValueCondition<TContentType, TConditionType, TDataType>(
            this IRuleBuilder<TContentType, TConditionType> ruleBuilder,
            TConditionType conditionType,
            Operators condOperator,
            TDataType operand)
        {
            var valueCondition = new ConditionNodeBuilder<TConditionType>()
                .AsValued(conditionType)
                .OfDataType<TDataType>()
                .WithComparisonOperator(condOperator)
                .SetOperand(operand)
                .Build();

            return ruleBuilder.WithCondition(valueCondition);
        }
    }
}