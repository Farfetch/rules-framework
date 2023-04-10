<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/NotEqualOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/NotEqualOperatorEvalStrategy.cs
{
    using System;

    internal sealed class NotEqualOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is IComparable leftOperandComparable && rightOperand is IComparable rightOperandComparable)
            {
                return leftOperandComparable.CompareTo(rightOperandComparable) != 0;
            }

            throw new NotSupportedException($"Only instances implementing {nameof(IComparable)} are supported.");
        }
    }
}