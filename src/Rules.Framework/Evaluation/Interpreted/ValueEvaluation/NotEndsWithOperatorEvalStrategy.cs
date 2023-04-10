<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/NotEndsWithOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/NotEndsWithOperatorEvalStrategy.cs
{
    using System;

    internal sealed class NotEndsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string && rightOperand is string)
            {
                var leftOperandAsString = leftOperand as string;
                var rightOperandAsString = rightOperand as string;

                return !leftOperandAsString.EndsWith(rightOperandAsString, StringComparison.Ordinal);
            }

            throw new NotSupportedException($"Only operands of type {nameof(String)} supported.");
        }
    }
}