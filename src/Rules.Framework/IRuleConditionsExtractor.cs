namespace Rules.Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the contract for extracting the conditions from rules.
    /// </summary>
    internal interface IRuleConditionsExtractor
    {
        /// <summary>
        /// Get the unique conditions associated with supplied <paramref name="rules"/>.
        /// </summary>
        /// <param name="rules"></param>
        /// <returns>the distinct collection of conditions contained on the set of rules.</returns>
        IEnumerable<string> GetConditions(IEnumerable<Rule> rules);
    }
}