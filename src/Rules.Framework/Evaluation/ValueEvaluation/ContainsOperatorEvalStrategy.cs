namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal sealed class ContainsOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.Contains(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'contains' comparison between operands of type '{leftOperand?.GetType().FullName}'.");
        }
    }
}