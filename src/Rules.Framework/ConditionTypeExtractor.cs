namespace Rules.Framework
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    /// <summary>
    /// Extracts Conditions Types from a Group of Rules.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class ConditionTypeExtractor<TContentType, TConditionType> : IConditionTypeExtractor<TContentType, TConditionType>
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
        public IEnumerable<TConditionType> GetConditionTypes(IEnumerable<Rule<TContentType, TConditionType>> matchedRules)
        {
            var conditionTypes = new List<TConditionType>();

            if (!matchedRules.Any())
            {
                return conditionTypes;
            }

            foreach (var rule in matchedRules)
            {
                var conditionType = rule.ContentContainer.GetContentAs<TConditionType>();

                if (!conditionTypes.Contains(conditionType))
                {
                    conditionTypes.Add(conditionType);
                }
            }

            return conditionTypes;
        }
    }
}