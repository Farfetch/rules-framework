namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The engine that holds the logic to match and manage rules.
    /// </summary>
    public interface IRulesEngine
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        IRulesEngineOptions Options { get; }

        /// <summary>
        /// Sets an existent inactive rule as active.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> ActivateRuleAsync(Rule rule);

        /// <summary>
        /// Adds a rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="ruleAddPriorityOption">The rule add priority option.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        Task<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption);

        /// <summary>
        /// Creates a content type.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">contentType</exception>
        Task<OperationResult> CreateContentTypeAsync(string contentType);

        /// <summary>
        /// Sets an existent active rule as inactive.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        Task<OperationResult> DeactivateRuleAsync(Rule rule);

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        Task<IEnumerable<string>> GetContentTypesAsync();

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
        Task<IEnumerable<string>> GetUniqueConditionTypesAsync(string contentType, DateTime dateBegin, DateTime dateEnd);

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
        Task<IEnumerable<Rule>> MatchManyAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions);

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
        Task<Rule> MatchOneAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions);

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
        Task<IEnumerable<Rule>> SearchAsync(SearchArgs<string, string> searchArgs);

        /// <summary>
        /// Updates an existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> UpdateRuleAsync(Rule rule);
    }
}