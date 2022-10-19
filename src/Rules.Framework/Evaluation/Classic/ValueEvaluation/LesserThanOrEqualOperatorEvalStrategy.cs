namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
{
    using System;

    internal sealed class LesserThanOrEqualOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is IComparable leftOperandComparable && rightOperand is IComparable rightOperandComparable)
            {
                return leftOperandComparable.CompareTo(rightOperandComparable) <= 0;
            }

            throw new NotSupportedException($"Only instances implementing {nameof(IComparable)} are supported.");
        }
    }
}