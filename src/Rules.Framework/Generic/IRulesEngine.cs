namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The engine that holds the logic to match and manage rules.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IRulesEngine<TContentType, TConditionType>
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        IRulesEngineOptions Options { get; }

        /// <summary>
        /// Activates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> ActivateRuleAsync(Rule<TContentType, TConditionType> rule);

        /// <summary>
        /// Adds a new rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="ruleAddPriorityOption">The rule add priority option.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule or rule</exception>
        /// <exception cref="NotSupportedException">The priority option is not supported.</exception>
        Task<OperationResult> AddRuleAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption);

        /// <summary>
        /// Creates a content type.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        Task CreateContentTypeAsync(TContentType contentType);

        /// <summary>
        /// Deactivates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> DeactivateRuleAsync(Rule<TContentType, TConditionType> rule);

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        Task<IEnumerable<TContentType>> GetContentTypesAsync();

        /// <summary>
        /// Get the unique condition types associated with rules of a specific content type.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, empty.</returns>
        Task<IEnumerable<TConditionType>> GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd);

        /// <summary>
        /// Provides all rule matches (if any) to the given content type at the specified <paramref
        /// name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="matchDateTime"></param>
        /// <param name="conditions"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchManyAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions);

        /// <summary>
        /// Provides a rule match (if any) to the given content type at the specified <paramref
        /// name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="matchDateTime"></param>
        /// <param name="conditions"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>
        /// If there's more than one match, a rule is selected based on the priority criteria and
        /// value: topmost selects the lowest priority number and bottommost selects highest priority.
        /// </para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions);

        /// <summary>
        /// Searches for rules on given content type that match on supplied <paramref name="searchArgs"/>.
        /// </summary>
        /// <param name="searchArgs"></param>
        /// <remarks>
        /// <para>
        /// Only the condition types supplied on input conditions are evaluated, the remaining
        /// conditions are ignored.
        /// </para>
        /// </remarks>
        /// <returns>the set of rules matching the conditions.</returns>
        Task<IEnumerable<Rule<TContentType, TConditionType>>> SearchAsync(SearchArgs<TContentType, TConditionType> searchArgs);

        /// <summary>
        /// Updates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> UpdateRuleAsync(Rule<TContentType, TConditionType> rule);
    }
}