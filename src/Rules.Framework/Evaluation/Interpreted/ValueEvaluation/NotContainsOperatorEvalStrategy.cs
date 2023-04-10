<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/NotContainsOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/NotContainsOperatorEvalStrategy.cs
{
    using System;

    internal sealed class NotContainsOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return !leftOperandAsString.Contains(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'not contains' comparison between operands of type '{leftOperand?.GetType().FullName}'.");
        }
    }
}