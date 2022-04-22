namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

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
                var rootCondition = rule.RootCondition;

                VisitConditionNode(rootCondition, conditionTypes);
            }

            return conditionTypes;
        }

        private static void VisitConditionNode(IConditionNode<TConditionType> conditionNode, List<TConditionType> conditionTypes)
        {
            switch (conditionNode)
            {
                case IValueConditionNode<TConditionType> valueConditionNode:

                    if (!conditionTypes.Contains(valueConditionNode.ConditionType))
                    {
                        conditionTypes.Add(valueConditionNode.ConditionType);
                    }

                    break;

                case ComposedConditionNode<TConditionType> composedConditionNode:

                    foreach (IConditionNode<TConditionType> childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        VisitConditionNode(childConditionNode, conditionTypes);
                    }

                    break;

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }
    }
}