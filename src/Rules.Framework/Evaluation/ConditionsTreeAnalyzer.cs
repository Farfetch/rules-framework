namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ConditionsTreeAnalyzer<TConditionType> : IConditionsTreeAnalyzer<TConditionType>
    {
        public bool AreAllSearchConditionsPresent(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions)
        {
            // Conditions checklist is a mere control construct to avoid a full sweep of the
            // condition nodes tree when we already found all conditions.
            var conditionsChecklist = new Dictionary<TConditionType, bool>(conditions.ToDictionary(ks => ks.Key, vs => false));

            return VisitConditionNode(conditionNode, conditionsChecklist);
        }

        private static bool VisitConditionNode(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, bool> conditionsChecklist)
        {
            switch (conditionNode)
            {
                case IValueConditionNode<TConditionType> valueConditionNode:
                    if (conditionsChecklist.ContainsKey(valueConditionNode.ConditionType))
                    {
                        conditionsChecklist[valueConditionNode.ConditionType] = true;
                    }

                    return conditionsChecklist.All(kvp => kvp.Value);

                case ComposedConditionNode<TConditionType> composedConditionNode:
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