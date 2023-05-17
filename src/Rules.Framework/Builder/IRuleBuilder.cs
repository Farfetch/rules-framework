namespace Rules.Framework
{
    using System;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    /// <summary>
    /// Builder to create a new rule.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IRuleBuilder<TContentType, TConditionType>
    {
        /// <summary>
        /// Builds the new rule.
        /// </summary>
        /// <returns></returns>
        RuleBuilderResult<TContentType, TConditionType> Build();

        /// <summary>
        /// Sets the new rule with the specified active status.
        /// </summary>
        /// <param name="active">The active status.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithActive(bool active);

        /// <summary>
        /// Sets the new rule with the specified condition.
        /// </summary>
        /// <param name="conditionFunc">
        /// The function with specific logic to create the condition for the rule.
        /// </param>
        /// <returns></returns>
        [Obsolete("This way of adding conditions is being deprecated. Please use a non-deprecated overload instead.")]
        IRuleBuilder<TContentType, TConditionType> WithCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc);

        /// <summary>
        /// Sets the new rule with the specified root condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode<TConditionType> condition);

        /// <summary>
        /// Sets the new rule with a root condition with the specified parameters.
        /// </summary>
        /// <typeparam name="TDataType">The type of the data type.</typeparam>
        /// <param name="conditionType">The content type.</param>
        /// <param name="condOperator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithCondition<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand);

        /// <summary>
        /// Sets the new rule with the specified root condition.
        /// </summary>
        /// <param name="conditionFunc">The condition func.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithCondition(
            Func<IRootConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc);

        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithContent(TContentType contentType, object content);

        /// <summary>
        /// Sets the new rule with the specified content container.
        /// </summary>
        /// <param name="contentContainer">The content container.</param>
        /// <returns></returns>
        [Obsolete("This way of building the content is being deprecated. Please use WithContent().")]
        IRuleBuilder<TContentType, TConditionType> WithContentContainer(ContentContainer<TContentType> contentContainer);

        /// <summary>
        /// Sets the new rule with the specified date begin.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithDateBegin(DateTime dateBegin);

        /// <summary>
        /// Sets the new rule with the specified dates interval.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithDatesInterval(DateTime dateBegin, DateTime? dateEnd);

        /// <summary>
        ///Sets the new rule with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IRuleBuilder<TContentType, TConditionType> WithName(string name);
    }
}