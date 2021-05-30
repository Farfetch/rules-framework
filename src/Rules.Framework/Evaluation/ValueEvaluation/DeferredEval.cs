namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;

    internal class DeferredEval : IDeferredEval
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

        public Func<IEnumerable<Condition<TConditionType>>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
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

        private bool Eval<TConditionNode, TConditionType, T>(IEnumerable<Condition<TConditionType>> conditions, TConditionNode valueConditionNode, MatchModes matchMode)
            where TConditionNode : ValueConditionNodeTemplate<T, TConditionType>
            where T : IComparable
            // To be removed on a future major release, when obsolete value condition nodes are removed.
            => this.Eval(conditions, valueConditionNode, valueConditionNode.Operand, matchMode);

        private bool Eval<TConditionType>(IEnumerable<Condition<TConditionType>> conditions, ValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode)
            => this.Eval(conditions, valueConditionNode, valueConditionNode.Operand, matchMode);

        private bool Eval<TConditionType>(IEnumerable<Condition<TConditionType>> conditions, IValueConditionNode<TConditionType> valueConditionNode, object rightOperand, MatchModes matchMode)
        {
            Condition<TConditionType> leftOperandCondition = conditions.FirstOrDefault(c => object.Equals(c.Type, valueConditionNode.ConditionType));

            if (leftOperandCondition is null)
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

            object leftOperand = leftOperandCondition?.Value;

            IConditionEvalDispatcher conditionEvalDispatcher = this.conditionEvalDispatchProvider.GetEvalDispatcher(leftOperand, valueConditionNode.Operator, rightOperand);

            return conditionEvalDispatcher.EvalDispatch(valueConditionNode.DataType, leftOperand, valueConditionNode.Operator, rightOperand);
        }
    }
}