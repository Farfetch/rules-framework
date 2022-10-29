namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.Specification;

    internal sealed class CompiledConditionsEvalEngine<TConditionType> : IConditionsEvalEngine<TConditionType>
    {
        private readonly IConditionsTreeCompiler<TConditionType> conditionsTreeCompiler;
        private readonly IMultiplicityEvaluator multiplicityEvaluator;
        private readonly RulesEngineOptions rulesEngineOptions;

        public CompiledConditionsEvalEngine(
            IConditionsTreeCompiler<TConditionType> conditionsTreeCompiler,
            IMultiplicityEvaluator multiplicityEvaluator,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsTreeCompiler = conditionsTreeCompiler;
            this.multiplicityEvaluator = multiplicityEvaluator;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public bool Eval(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            if (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompiledFlagKey, out var compiledFlag) || !(bool)compiledFlag)
            {
                this.conditionsTreeCompiler.Compile(conditionNode);
            }

            ISpecification<IDictionary<TConditionType, object>> specification = this.BuildSpecification(conditionNode, conditions, evaluationOptions.MatchMode);

            return specification.IsSatisfiedBy(conditions);
        }

        private static bool AreAllSearchConditionsPresent(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions)
        {
            // Conditions checklist is a mere control construct to avoid a full sweep of the
            // condition nodes tree when we already found all conditions.
            IDictionary<TConditionType, bool> conditionsChecklist = new Dictionary<TConditionType, bool>(conditions.ToDictionary(ks => ks.Key, vs => false));

            return VisitConditionNode(conditionNode, conditionsChecklist);
        }

        private static bool VisitConditionNode(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, bool> conditionsChecklist)
        {
            switch (conditionNode)
            {
                case ValueConditionNode<TConditionType> valueConditionNode:
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

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecification(
            IConditionNode<TConditionType> conditionNode,
            IDictionary<TConditionType, object> conditions,
            MatchModes matchMode)
        {
            return conditionNode switch
            {
                ValueConditionNode<TConditionType> valueConditionNode => this.BuildSpecificationForValueNode(valueConditionNode, conditions, matchMode),
                ComposedConditionNode<TConditionType> composedConditionNode => this.BuildSpecificationForComposedNode(composedConditionNode, conditions, matchMode),
                _ => throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'."),
            };
        }

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecificationForComposedNode(
            ComposedConditionNode<TConditionType> composedConditionNode,
            IDictionary<TConditionType, object> conditions,
            MatchModes matchMode)
        {
            IEnumerable<ISpecification<IDictionary<TConditionType, object>>> childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn, conditions, matchMode));

            return composedConditionNode.LogicalOperator switch
            {
                LogicalOperators.And => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2)),
                LogicalOperators.Or => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2)),
                _ => throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'."),
            };
        }

        private ISpecification<IDictionary<TConditionType, object>> BuildSpecificationForValueNode(
            ValueConditionNode<TConditionType> valueConditionNode,
            IDictionary<TConditionType, object> conditions,
            MatchModes matchMode)
        {
            conditions.TryGetValue(valueConditionNode.ConditionType, out object leftOperand);

            if (leftOperand is null)
            {
                if (this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard)
                {
                    return new FuncSpecification<IDictionary<TConditionType, object>>(conditions => false);
                }
                else if (matchMode == MatchModes.Search)
                {
                    // When match mode is search, if condition is missing, it is not used as search
                    // criteria, so we don't filter out the rule.
                    return new FuncSpecification<IDictionary<TConditionType, object>>(conditions => true);
                }
            }

            string multiplicity = this.multiplicityEvaluator.EvaluateMultiplicity(leftOperand, valueConditionNode.Operator, valueConditionNode.Operand);

            valueConditionNode.Properties.TryGetValue(ConditionNodeProperties.GetCompiledDelegateKey(multiplicity), out object conditionFuncAux);
            Func<IDictionary<TConditionType, object>, bool> conditionFunc = conditionFuncAux as Func<IDictionary<TConditionType, object>, bool>;

            return new FuncSpecification<IDictionary<TConditionType, object>>(conditionFunc);
        }
    }
}