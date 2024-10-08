namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;

    /// <summary>
    /// Defines a extractor of conditions from rules.
    /// </summary>
    public class RuleConditionsExtractor : IRuleConditionsExtractor
    {
        /// <inheritdoc/>
        public IEnumerable<string> GetConditions(IEnumerable<Rule> matchedRules)
        {
            var conditions = new HashSet<string>(StringComparer.Ordinal);

            if (!matchedRules.Any())
            {
                return conditions;
            }

            foreach (var rootCondition in matchedRules.Select(r => r.RootCondition))
            {
                if (rootCondition is null)
                {
                    continue;
                }

                VisitConditionNode(rootCondition, conditions);
            }

            return conditions;
        }

        private static void VisitConditionNode(IConditionNode conditionNode, HashSet<string> conditions)
        {
            switch (conditionNode)
            {
                case IValueConditionNode valueConditionNode:

                    conditions.Add(valueConditionNode.Condition);
                    break;

                case ComposedConditionNode composedConditionNode:

                    foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        VisitConditionNode(childConditionNode, conditions);
                    }

                    break;

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }
    }
}