namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections;
    using Rules.Framework;

    internal sealed class MultiplicityEvaluator : IMultiplicityEvaluator
    {
        public static string Evaluate(object leftOperand, Operators @operator, object rightOperand) => leftOperand switch
        {
            IEnumerable when leftOperand is not string && rightOperand is IEnumerable && rightOperand is not string => Multiplicities.ManyToMany,
            IEnumerable when leftOperand is not string => Multiplicities.ManyToOne,
            object when rightOperand is IEnumerable && rightOperand is not string => Multiplicities.OneToMany,
            object => Multiplicities.OneToOne,
            null when OperatorSupportsOneMultiplicityLeftOperand(@operator) && rightOperand is IEnumerable && rightOperand is not string => Multiplicities.OneToMany,
            null when OperatorSupportsOneMultiplicityLeftOperand(@operator) => Multiplicities.OneToOne,
            _ => throw new NotSupportedException($"Could not evaluate multiplicity. [Left Operand: {leftOperand} | Operator: {@operator} | Right Operand: {rightOperand}]")
        };

        public string EvaluateMultiplicity(object leftOperand, Operators @operator, object rightOperand) => Evaluate(leftOperand, @operator, rightOperand);

        private static bool OperatorSupportsOneMultiplicityLeftOperand(Operators @operator)
        {
            if (!OperatorsMetadata.AllByOperator.TryGetValue(@operator, out var operatorMetadata))
            {
                return false;
            }

            return operatorMetadata.HasSupportForOneMultiplicityAtLeft;
        }
    }
}