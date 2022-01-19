namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal class EndsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.EndsWith(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'startswith' comparison between operands of type '{leftOperand?.GetType().FullName}'.");
        }
    }
}