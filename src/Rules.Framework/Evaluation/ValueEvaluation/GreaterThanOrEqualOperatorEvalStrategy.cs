using System;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal class GreaterThanOrEqualOperatorEvalStrategy : IOperatorEvalStrategy
    {
        public bool Eval<T>(T leftOperand, T rightOperand) where T : IComparable<T>
        {
            return leftOperand.CompareTo(rightOperand) >= 0;
        }
    }
}