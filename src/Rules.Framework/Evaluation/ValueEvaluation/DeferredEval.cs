using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Framework.Core.ConditionNodes;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
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

        public Func<IEnumerable<Condition<TConditionType>>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode)
        {
            switch (valueConditionNode)
            {
                case IntegerConditionNode<TConditionType> integerConditionNode:
                    return (conditions) => Eval<IntegerConditionNode<TConditionType>, TConditionType, int>(conditions, integerConditionNode);

                case DecimalConditionNode<TConditionType> decimalConditionNode:
                    return (conditions) => Eval<DecimalConditionNode<TConditionType>, TConditionType, decimal>(conditions, decimalConditionNode);

                case StringConditionNode<TConditionType> stringConditionNode:
                    return (conditions) => Eval<StringConditionNode<TConditionType>, TConditionType, string>(conditions, stringConditionNode);

                case BooleanConditionNode<TConditionType> booleanConditionNode:
                    return (conditions) => Eval<BooleanConditionNode<TConditionType>, TConditionType, bool>(conditions, booleanConditionNode);

                default:
                    throw new NotSupportedException($"Unsupported value condition node: '{valueConditionNode.GetType().Name}'.");
            }
        }

        private bool Eval<TConditionNode, TConditionType, T>(IEnumerable<Condition<TConditionType>> conditions, TConditionNode valueConditionNode)
            where TConditionNode : ValueConditionNodeTemplate<T, TConditionType>
            where T : IComparable<T>
        {
            Condition<TConditionType> leftOperandCondition = conditions.FirstOrDefault(c => object.Equals(c.Type, valueConditionNode.ConditionType));

            if (leftOperandCondition == null && this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard)
            {
                return false;
            }

            T leftOperand = (T)Convert.ChangeType(leftOperandCondition?.Value ?? this.rulesEngineOptions.DataTypeDefaults[valueConditionNode.DataType], typeof(T));
            T rightOperand = valueConditionNode.Operand;
            IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(valueConditionNode.Operator);

            return operatorEvalStrategy.Eval(leftOperand, rightOperand);
        }
    }
}