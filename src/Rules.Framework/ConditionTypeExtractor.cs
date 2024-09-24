namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;

    /// <summary>
    /// Extracts Conditions Types from a Group of Rules.
    /// </summary>
    public class ConditionTypeExtractor : IConditionTypeExtractor
    {
        /// <summary>
        /// Get the unique condition types associated with rules of a specific content type.
        /// </summary>
        /// <param name="matchedRules"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, empty.</returns>
        public IEnumerable<string> GetConditionTypes(IEnumerable<Rule> matchedRules)
        {
            var conditionTypes = new HashSet<string>(StringComparer.Ordinal);

            if (!matchedRules.Any())
            {
                return conditionTypes;
            }

            foreach (var rootCondition in matchedRules.Select(r => r.RootCondition))
            {
                if (rootCondition is null)
                {
                    continue;
                }

                VisitConditionNode(rootCondition, conditionTypes);
            }

            return conditionTypes;
        }

        private static void VisitConditionNode(IConditionNode conditionNode, HashSet<string> conditionTypes)
        {
            switch (conditionNode)
            {
                case IValueConditionNode valueConditionNode:

                    conditionTypes.Add(valueConditionNode.ConditionType);
                    break;

                case ComposedConditionNode composedConditionNode:

                    foreach (IConditionNode childConditionNode in composedConditionNode.ChildConditionNodes)
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