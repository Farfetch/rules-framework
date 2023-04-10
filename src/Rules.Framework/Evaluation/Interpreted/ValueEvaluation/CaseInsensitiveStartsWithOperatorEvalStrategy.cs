<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/CaseInsensitiveStartsWithOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/CaseInsensitiveStartsWithOperatorEvalStrategy.cs
{
    using System;
    using System.Globalization;

    internal sealed class CaseInsensitiveStartsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string && rightOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.StartsWith(rightOperandAsString, ignoreCase: true, culture: CultureInfo.InvariantCulture);
            }

            throw new NotSupportedException($"Unsupported 'caseinsensitivestartswith' comparison between operands of type '{leftOperand?.GetType().FullName}' and '{rightOperand?.GetType().FullName}'.");
        }
    }
}