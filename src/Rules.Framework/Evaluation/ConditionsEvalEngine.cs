namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.Specification;
    using Rules.Framework.Evaluation.ValueEvaluation;

    internal class ConditionsEvalEngine<TConditionType> : IConditionsEvalEngine<TConditionType>
    {
        private readonly IDeferredEval deferredEval;

        public ConditionsEvalEngine(IDeferredEval deferredEval)
        {
            this.deferredEval = deferredEval;
        }

        public bool Eval(IConditionNode<TConditionType> conditionNode, IEnumerable<Condition<TConditionType>> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            ISpecification<IEnumerable<Condition<TConditionType>>> specification = this.BuildSpecification(conditionNode, evaluationOptions.MatchMode);

            return specification.IsSatisfiedBy(conditions);
        }

        private static bool AreAllSearchConditionsPresent(IConditionNode<TConditionType> conditionNode, IEnumerable<Condition<TConditionType>> conditions)
        {
            // Conditions checklist is a mere control construct to avoid a full sweep of the condition nodes tree when we already found all conditions.
            IDictionary<TConditionType, bool> conditionsChecklist = new Dictionary<TConditionType, bool>(conditions.ToDictionary(ks => ks.Type, vs => false));

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
                    foreach (IConditionNode<TConditionType> childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        bool allPresentAlready = VisitConditionNode(childConditionNode, conditionsChecklist);
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

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecification(IConditionNode<TConditionType> conditionNode, MatchModes matchMode)
        {
            return conditionNode switch
            {
                IValueConditionNode<TConditionType> valueConditionNode => this.BuildSpecificationForValueNode(valueConditionNode, matchMode),
                ComposedConditionNode<TConditionType> composedConditionNode => this.BuildSpecificationForComposedNode(composedConditionNode, matchMode),
                _ => throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'."),
            };
        }

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecificationForComposedNode(ComposedConditionNode<TConditionType> composedConditionNode, MatchModes matchMode)
        {
            IEnumerable<ISpecification<IEnumerable<Condition<TConditionType>>>> childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn, matchMode));

            return composedConditionNode.LogicalOperator switch
            {
                LogicalOperators.And => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2)),
                LogicalOperators.Or => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2)),
                _ => throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'."),
            };
        }

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecificationForValueNode(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
        {
            return new FuncSpecification<IEnumerable<Condition<TConditionType>>>(this.deferredEval.GetDeferredEvalFor(valueConditionNode, matchMode));
        }
    }
}