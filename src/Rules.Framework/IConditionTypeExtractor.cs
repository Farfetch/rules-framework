namespace Rules.Framework
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    /// <summary>
    /// Extracts Conditions Types from a Group of Rules.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public interface IConditionTypeExtractor<TContentType, TConditionType>
    {
        /// <summary>
        /// Get the unique condition types associated with rules of a specific content type/>.
        /// </summary>
        /// <param name="matchedRules"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        IEnumerable<TConditionType> GetConditionTypes(IEnumerable<Rule<TContentType, TConditionType>> matchedRules);
    }
}