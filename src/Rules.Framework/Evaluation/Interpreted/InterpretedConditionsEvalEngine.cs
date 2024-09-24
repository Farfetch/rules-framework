namespace Rules.Framework.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;

    internal sealed class InterpretedConditionsEvalEngine : IConditionsEvalEngine
    {
        private readonly IConditionsTreeAnalyzer conditionsTreeAnalyzer;
        private readonly IDeferredEval deferredEval;

        public InterpretedConditionsEvalEngine(
            IDeferredEval deferredEval,
            IConditionsTreeAnalyzer conditionsTreeAnalyzer)
        {
            this.deferredEval = deferredEval;
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
        }

        public bool Eval(IConditionNode conditionNode, IDictionary<string, object> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            ISpecification<IDictionary<string, object>> specification = this.BuildSpecification(conditionNode, evaluationOptions.MatchMode);

            return specification.IsSatisfiedBy(conditions);
        }

        private ISpecification<IDictionary<string, object>> BuildSpecification(IConditionNode conditionNode, MatchModes matchMode)
        {
            return conditionNode switch
            {
                IValueConditionNode valueConditionNode => this.BuildSpecificationForValueNode(valueConditionNode, matchMode),
                ComposedConditionNode composedConditionNode => this.BuildSpecificationForComposedNode(composedConditionNode, matchMode),
                _ => throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'."),
            };
        }

        private ISpecification<IDictionary<string, object>> BuildSpecificationForComposedNode(ComposedConditionNode composedConditionNode, MatchModes matchMode)
        {
            IEnumerable<ISpecification<IDictionary<string, object>>> childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn, matchMode));

            return composedConditionNode.LogicalOperator switch
            {
                LogicalOperators.And => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2)),
                LogicalOperators.Or => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2)),
                _ => throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'."),
            };
        }

        private ISpecification<IDictionary<string, object>> BuildSpecificationForValueNode(IValueConditionNode valueConditionNode, MatchModes matchMode)
        {
            return new FuncSpecification<IDictionary<string, object>>(this.deferredEval.GetDeferredEvalFor(valueConditionNode, matchMode));
        }
    }
}