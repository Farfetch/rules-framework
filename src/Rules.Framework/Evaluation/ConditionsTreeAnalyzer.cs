namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.ConditionNodes;

    internal sealed class ConditionsTreeAnalyzer : IConditionsTreeAnalyzer
    {
        public bool AreAllSearchConditionsPresent(IConditionNode conditionNode, IDictionary<string, object> conditions)
        {
            // Conditions checklist is a mere control construct to avoid a full sweep of the
            // condition nodes tree when we already found all conditions.
            var conditionsChecklist = conditions.ToDictionary(ks => ks.Key, vs => false, StringComparer.Ordinal);

            return VisitConditionNode(conditionNode, conditionsChecklist);
        }

        private static bool VisitConditionNode(IConditionNode conditionNode, IDictionary<string, bool> conditionsChecklist)
        {
            switch (conditionNode)
            {
                case IValueConditionNode valueConditionNode:
                    if (conditionsChecklist.ContainsKey(valueConditionNode.Condition))
                    {
                        conditionsChecklist[valueConditionNode.Condition] = true;
                    }

                    return conditionsChecklist.All(kvp => kvp.Value);

                case ComposedConditionNode composedConditionNode:
                    foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        var allPresentAlready = VisitConditionNode(childConditionNode, conditionsChecklist);
                        if (allPresentAlready)
                        {
                            return true;
                        }
                    }

                    return false;

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }
    }
}