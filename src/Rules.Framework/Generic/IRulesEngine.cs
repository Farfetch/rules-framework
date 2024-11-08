namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The engine that holds the logic to match and manage rules.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRulesEngine<TRuleset, TCondition>
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
        Task<OperationResult> ActivateRuleAsync(Rule<TRuleset, TCondition> rule);

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
        Task<OperationResult> AddRuleAsync(Rule<TRuleset, TCondition> rule, RuleAddPriorityOption ruleAddPriorityOption);

        /// <summary>
        /// Creates a ruleset.
        /// </summary>
        /// <param name="ruleset">the ruleset name.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">ruleset</exception>
        Task CreateRulesetAsync(TRuleset ruleset);

        /// <summary>
        /// Deactivates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> DeactivateRuleAsync(Rule<TRuleset, TCondition> rule);

        /// <summary>
        /// Gets the rulesets.
        /// </summary>
        /// <returns>a collection of all rulesets, including respective metadata.</returns>
        Task<IEnumerable<Ruleset<TRuleset>>> GetRulesetsAsync();

        /// <summary>
        /// Get the unique conditions associated with rules of a specific ruleset.
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are fetched and a
        /// distinct collection of all conditions is returned.
        /// </para>
        /// </remarks>
        /// <returns>
        /// the distinct collection of all conditions present on the rules of the provided ruleset.
        /// </returns>
        Task<IEnumerable<TCondition>> GetUniqueConditionsAsync(TRuleset ruleset, DateTime dateBegin, DateTime dateEnd);

        /// <summary>
        /// Provides all rule matches (if any) to the given <paramref name="ruleset"/> at the
        /// specified <paramref name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="ruleset"></param>
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
        Task<IEnumerable<Rule<TRuleset, TCondition>>> MatchManyAsync(TRuleset ruleset, DateTime matchDateTime, IDictionary<TCondition, object> conditions);

        /// <summary>
        /// Provides a rule match (if any) to the given <paramref name="ruleset"/> at the specified
        /// <paramref name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="ruleset"></param>
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
        Task<Rule<TRuleset, TCondition>> MatchOneAsync(TRuleset ruleset, DateTime matchDateTime, IDictionary<TCondition, object> conditions);

        /// <summary>
        /// Searches for rules that match on supplied <paramref name="searchArgs"/>.
        /// </summary>
        /// <param name="searchArgs"></param>
        /// <remarks>
        /// <para>
        /// Only the conditions supplied on input conditions are evaluated, the remaining conditions
        /// are ignored.
        /// </para>
        /// </remarks>
        /// <returns>the set of rules matching the conditions.</returns>
        Task<IEnumerable<Rule<TRuleset, TCondition>>> SearchAsync(SearchArgs<TRuleset, TCondition> searchArgs);

        /// <summary>
        /// Updates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>
        /// the operation result, containing success/failure indication and messages associated to
        /// errors occurred during the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        Task<OperationResult> UpdateRuleAsync(Rule<TRuleset, TCondition> rule);
    }
}