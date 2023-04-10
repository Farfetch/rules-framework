<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/StartsWithOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/StartsWithOperatorEvalStrategy.cs
{
    using System;

    internal sealed class StartsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string && rightOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.StartsWith(rightOperandAsString, StringComparison.Ordinal);
            }

            throw new NotSupportedException($"Unsupported 'startswith' comparison between operands of type '{leftOperand?.GetType().FullName}' and '{rightOperand?.GetType().FullName}'.");
        }
    }
}