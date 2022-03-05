namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;
    using System.Globalization;

    internal class CaseInsensitiveStartsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
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