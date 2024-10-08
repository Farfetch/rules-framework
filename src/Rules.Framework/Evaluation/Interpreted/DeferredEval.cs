namespace Rules.Framework.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers;

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

        public Func<IDictionary<string, object>, bool> GetDeferredEvalFor(IValueConditionNode valueConditionNode, MatchModes matchMode)
            => (conditions) => Eval(conditions, valueConditionNode, matchMode);

        private bool Eval(IDictionary<string, object> conditions, IValueConditionNode valueConditionNode, MatchModes matchMode)
        {
            var rightOperand = valueConditionNode.Operand;

            conditions.TryGetValue(valueConditionNode.Condition, out var leftOperand);

            if (leftOperand is null)
            {
                if (this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard)
                {
                    return false;
                }

                if (matchMode == MatchModes.Search)
                {
                    // When match mode is search, if condition is missing, it is not used as search
                    // criteria, so we don't filter out the rule.
                    return true;
                }
            }

            var conditionEvalDispatcher = this.conditionEvalDispatchProvider.GetEvalDispatcher(leftOperand!, valueConditionNode.Operator, rightOperand);

            return conditionEvalDispatcher.EvalDispatch(valueConditionNode.DataType, leftOperand!, valueConditionNode.Operator, rightOperand);
        }
    }
}