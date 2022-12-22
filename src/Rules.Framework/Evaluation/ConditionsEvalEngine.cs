namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.Specification;
    using Rules.Framework.Evaluation.ValueEvaluation;

    internal sealed class ConditionsEvalEngine<TConditionType> : IConditionsEvalEngine<TConditionType>
    {
        private readonly IConditionsTreeAnalyzer<TConditionType> conditionsTreeAnalyzer;
        private readonly IDeferredEval deferredEval;

        public ConditionsEvalEngine(
            IDeferredEval deferredEval,
            IConditionsTreeAnalyzer<TConditionType> conditionsTreeAnalyzer)
        {
            this.deferredEval = deferredEval;
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
        }

        public bool Eval(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            ISpecification<IDictionary<TConditionType, object>> specification = this.BuildSpecification(conditionNode, evaluationOptions.MatchMode);

            return specification.IsSatisfiedBy(conditions);
        }

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecification(IConditionNode<TConditionType> conditionNode, MatchModes matchMode)
        {
            return conditionNode switch
            {
                IValueConditionNode<TConditionType> valueConditionNode => this.BuildSpecificationForValueNode(valueConditionNode, matchMode),
                ComposedConditionNode<TConditionType> composedConditionNode => this.BuildSpecificationForComposedNode(composedConditionNode, matchMode),
                _ => throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'."),
            };
        }

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecificationForComposedNode(ComposedConditionNode<TConditionType> composedConditionNode, MatchModes matchMode)
        {
            IEnumerable<ISpecification<IDictionary<TConditionType, object>>> childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn, matchMode));

            return composedConditionNode.LogicalOperator switch
            {
                LogicalOperators.And => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2)),
                LogicalOperators.Or => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2)),
                _ => throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'."),
            };
        }

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecificationForValueNode(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
        {
            return new FuncSpecification<IDictionary<TConditionType, object>>(this.deferredEval.GetDeferredEvalFor(valueConditionNode, matchMode));
        }
    }
}