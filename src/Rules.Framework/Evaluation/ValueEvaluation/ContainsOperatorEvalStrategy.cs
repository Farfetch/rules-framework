namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal class ContainsOperatorEvalStrategy : IOperatorEvalStrategy
    {
        public bool Eval<T>(T leftOperand, T rightOperand)
            where T : IComparable<T>
        {
            if (leftOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.Contains(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'contains' comparison between operands of type '{typeof(T).FullName}'.");
        }
    }
}