using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Framework.Core.ConditionNodes;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal class DeferredEval : IDeferredEval
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public DeferredEval(IOperatorEvalStrategyFactory operatorEvalStrategyFactory)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public Func<IEnumerable<Condition<TConditionType>>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode)
        {
            switch (valueConditionNode)
            {
                case IntegerConditionNode<TConditionType> integerConditionNode:
                    return (conditions) =>
                    {
                        int leftOperand = SeekAndConvert<int, TConditionType>(integerConditionNode.ConditionType, conditions);
                        int rightOperand = integerConditionNode.Operand;
                        IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(integerConditionNode.Operator);

                        return operatorEvalStrategy.Eval(leftOperand, rightOperand);
                    };

                case DecimalConditionNode<TConditionType> decimalConditionNode:
                    return (conditions) =>
                    {
                        decimal leftOperand = SeekAndConvert<decimal, TConditionType>(decimalConditionNode.ConditionType, conditions);
                        decimal rightOperand = decimalConditionNode.Operand;
                        IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(decimalConditionNode.Operator);

                        return operatorEvalStrategy.Eval(leftOperand, rightOperand);
                    };

                case StringConditionNode<TConditionType> stringConditionNode:
                    return (conditions) =>
                    {
                        string leftOperand = SeekAndConvert<string, TConditionType>(stringConditionNode.ConditionType, conditions);
                        string rightOperand = stringConditionNode.Operand;
                        IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(stringConditionNode.Operator);

                        return operatorEvalStrategy.Eval(leftOperand, rightOperand);
                    };

                case BooleanConditionNode<TConditionType> booleanConditionNode:
                    return (conditions) =>
                    {
                        bool leftOperand = SeekAndConvert<bool, TConditionType>(booleanConditionNode.ConditionType, conditions);
                        bool rightOperand = booleanConditionNode.Operand;
                        IOperatorEvalStrategy operatorEvalStrategy = operatorEvalStrategyFactory.GetOperatorEvalStrategy(booleanConditionNode.Operator);

                        return operatorEvalStrategy.Eval(leftOperand, rightOperand);
                    };

                default:
                    throw new NotSupportedException($"Unsupported value condition node: '{valueConditionNode.GetType().Name}'.");
            }
        }

        private static T SeekAndConvert<T, TConditionType>(TConditionType conditionType, IEnumerable<Condition<TConditionType>> conditions)
        {
            Condition<TConditionType> condition = conditions.FirstOrDefault(c => object.Equals(c.Type, conditionType));

            return (T)Convert.ChangeType(condition.Value, typeof(T));
        }
    }
}