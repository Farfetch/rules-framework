namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Serialization;

    /// <summary>
    /// Builder to create a new rule.
    /// </summary>
    public interface IRuleBuilder
    {
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

        /// <summary>
        /// Sets the new rule with the specified root condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        IRuleBuilder WithCondition(IConditionNode condition);

        /// <summary>
        /// Sets the new rule with a value condition with the specified parameters.
        /// </summary>
        /// <typeparam name="TDataType">The type of the data type.</typeparam>
        /// <param name="conditionType">The content type.</param>
        /// <param name="condOperator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        IRuleBuilder WithCondition<TDataType>(string conditionType, Operators condOperator, TDataType operand);

        /// <summary>
        /// Sets the new rule with the specified root condition.
        /// </summary>
        /// <param name="conditionFunc">The condition func.</param>
        /// <returns></returns>
        IRuleBuilder WithCondition(
            Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc);

        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        IRuleBuilder WithContent(string contentType, object content);

        /// <summary>
        /// Sets the new rule with the specified date begin.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        IRuleBuilder WithDateBegin(DateTime dateBegin);

        /// <summary>
        /// Sets the new rule with the specified dates interval.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        IRuleBuilder WithDatesInterval(DateTime dateBegin, DateTime? dateEnd);

        /// <summary>
        ///Sets the new rule with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IRuleBuilder WithName(string name);

        /// <summary>
        /// Sets the new rule with the specified serialized content.
        /// </summary>
        /// <param name="contentType">The type of the content.</param>
        /// <param name="serializedContent">The serialized content.</param>
        /// <param name="contentSerializationProvider">The content serialization provider.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">ruleBuilder or contentSerializationProvider</exception>
        IRuleBuilder WithSerializedContent(
            string contentType,
            object serializedContent,
            IContentSerializationProvider contentSerializationProvider);
    }
}