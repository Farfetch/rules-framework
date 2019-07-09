namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal class GreaterThanOrEqualOperatorEvalStrategy : IOperatorEvalStrategy
    {
        public bool Eval<T>(T leftOperand, T rightOperand) where T : IComparable<T>
        {
            return leftOperand.CompareTo(rightOperand) >= 0;
        }
    }
}