namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

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

        Task<IEnumerable<string>> GetContentTypesAsync();

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
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        Task UpdateRuleAsync(Rule rule);
    }
}