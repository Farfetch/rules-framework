namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.Core.ConditionNodes;

    internal class DeferredEval : IDeferredEval
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;
        private readonly RulesEngineOptions rulesEngineOptions;

        public DeferredEval(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            RulesEngineOptions rulesEngineOptions)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
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
                _ => throw new NotSupportedException($"Unsupported value condition node: '{valueConditionNode.GetType().Name}'."),
            };
        }

        private bool Eval<TConditionNode, TConditionType, T>(IEnumerable<Condition<TConditionType>> conditions, TConditionNode valueConditionNode, MatchModes matchMode)
            where TConditionNode : ValueConditionNodeTemplate<T, TConditionType>
            where T : IComparable<T>
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
                    // When match mode is search, if condition is missing, it is not used as search criteria, so we don't filter out the rule.
                    return true;
                }
            }

            T leftOperand = (T)Convert.ChangeType(leftOperandCondition?.Value
                ?? this.rulesEngineOptions.DataTypeDefaults[valueConditionNode.DataType], typeof(T), CultureInfo.InvariantCulture);
            T rightOperand = valueConditionNode.Operand;
            IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(valueConditionNode.Operator);

            return operatorEvalStrategy.Eval(leftOperand, rightOperand);
        }
    }
}