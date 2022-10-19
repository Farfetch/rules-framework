namespace Rules.Framework.Evaluation.Classic
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers;

    internal sealed class DeferredEval : IDeferredEval
    {
        private readonly IConditionEvalDispatchProvider conditionEvalDispatchProvider;
        private readonly RulesEngineOptions rulesEngineOptions;

        public DeferredEval(
            IConditionEvalDispatchProvider conditionEvalDispatchProvider,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionEvalDispatchProvider = conditionEvalDispatchProvider;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public Func<IDictionary<TConditionType, object>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
        {
            return valueConditionNode switch
            {
                IntegerConditionNode<TConditionType> integerConditionNode => (conditions) => Eval<IntegerConditionNode<TConditionType>, TConditionType, int>(conditions, integerConditionNode, matchMode),
                DecimalConditionNode<TConditionType> decimalConditionNode => (conditions) => Eval<DecimalConditionNode<TConditionType>, TConditionType, decimal>(conditions, decimalConditionNode, matchMode),
                StringConditionNode<TConditionType> stringConditionNode => (conditions) => Eval<StringConditionNode<TConditionType>, TConditionType, string>(conditions, stringConditionNode, matchMode),
                BooleanConditionNode<TConditionType> booleanConditionNode => (conditions) => Eval<BooleanConditionNode<TConditionType>, TConditionType, bool>(conditions, booleanConditionNode, matchMode),
                ValueConditionNode<TConditionType> valueConditionNodeImpl => (conditions) => Eval(conditions, valueConditionNodeImpl, matchMode),
                _ => throw new NotSupportedException($"Unsupported value condition node: '{valueConditionNode.GetType().Name}'."),
            };
        }

        private bool Eval<TConditionNode, TConditionType, T>(IDictionary<TConditionType, object> conditions, TConditionNode valueConditionNode, MatchModes matchMode)
            where TConditionNode : ValueConditionNodeTemplate<T, TConditionType>
            where T : IComparable
            // To be removed on a future major release, when obsolete value condition nodes are removed.
            => this.Eval(conditions, valueConditionNode, valueConditionNode.Operand, matchMode);

        private bool Eval<TConditionType>(IDictionary<TConditionType, object> conditions, ValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
            => this.Eval(conditions, valueConditionNode, valueConditionNode.Operand, matchMode);

        private bool Eval<TConditionType>(IDictionary<TConditionType, object> conditions, IValueConditionNode<TConditionType> valueConditionNode, object rightOperand, MatchModes matchMode)
        {
            conditions.TryGetValue(valueConditionNode.ConditionType, out var leftOperand);

            if (leftOperand is null)
            {
                if (this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard)
                {
                    return false;
                }
                else if (matchMode == MatchModes.Search)
                {
                    // When match mode is search, if condition is missing, it is not used as search
                    // criteria, so we don't filter out the rule.
                    return true;
                }
            }

            IConditionEvalDispatcher conditionEvalDispatcher = this.conditionEvalDispatchProvider.GetEvalDispatcher(leftOperand, valueConditionNode.Operator, rightOperand);

            return conditionEvalDispatcher.EvalDispatch(valueConditionNode.DataType, leftOperand, valueConditionNode.Operator, rightOperand);
        }
    }
}