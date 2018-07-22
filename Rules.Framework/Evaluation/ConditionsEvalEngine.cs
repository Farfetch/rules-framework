using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Framework.Core;
using Rules.Framework.Core.ConditionNodes;
using Rules.Framework.Evaluation.Specification;
using Rules.Framework.Evaluation.ValueEvaluation;

namespace Rules.Framework.Evaluation
{
    internal class ConditionsEvalEngine<TConditionType> : IConditionsEvalEngine<TConditionType>
    {
        private readonly IDeferredEval deferredEval;

        public ConditionsEvalEngine(IDeferredEval deferredEval)
        {
            this.deferredEval = deferredEval;
        }

        public bool Eval(IConditionNode<TConditionType> conditionNode, IEnumerable<Condition<TConditionType>> conditions)
        {
            ISpecification<IEnumerable<Condition<TConditionType>>> specification = this.BuildSpecification(conditionNode);

            return specification.IsSatisfiedBy(conditions);
        }

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecification(IConditionNode<TConditionType> conditionNode)
        {
            switch (conditionNode)
            {
                case IValueConditionNode<TConditionType> valueConditionNode:
                    return this.BuildSpecificationForValueNode(valueConditionNode);

                case ComposedConditionNode<TConditionType> composedConditionNode:
                    return this.BuildSpecificationForComposedNode(composedConditionNode);

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecificationForComposedNode(ComposedConditionNode<TConditionType> composedConditionNode)
        {
            IEnumerable<ISpecification<IEnumerable<Condition<TConditionType>>>> childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn));

            switch (composedConditionNode.LogicalOperator)
            {
                case LogicalOperators.And:
                    return childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2));

                case LogicalOperators.Or:
                    return childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2));

                default:
                    throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'.");
            }
        }

        private ISpecification<IEnumerable<Condition<TConditionType>>> BuildSpecificationForValueNode(IValueConditionNode<TConditionType> valueConditionNode)
        {
            return new FuncSpecification<IEnumerable<Condition<TConditionType>>>(this.deferredEval.GetDeferredEvalFor(valueConditionNode));
        }
    }
}