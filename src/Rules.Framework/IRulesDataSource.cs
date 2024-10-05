namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the interface contract for a rules data source for specified content type.
    /// </summary>
    public interface IRulesDataSource
    {
        /// <summary>
        /// Adds a new rule to data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        Task AddRuleAsync(Rule rule);

        /// <summary>
        /// Creates a new ruleset on the data source.
        /// </summary>
        /// <param name="ruleset">the ruleset name.</param>
        /// <returns></returns>
        Task CreateRulesetAsync(string ruleset);

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="contentType"/> between
        /// <paramref name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="contentType">the content type categorization.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        Task<IEnumerable<Rule>> GetRulesAsync(string contentType, DateTime dateBegin, DateTime dateEnd);

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        Task<IEnumerable<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs);

        /// <summary>
        /// Gets the rulesets from the data source.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Ruleset>> GetRulesetsAsync();

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        Task UpdateRuleAsync(Rule rule);
    }
}