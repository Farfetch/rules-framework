namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal class LesserThanOrEqualOperatorEvalStrategy : IOperatorEvalStrategy
    {
        public bool Eval<T>(T leftOperand, T rightOperand) where T : IComparable<T>
        {
            return leftOperand.CompareTo(rightOperand) <= 0;
        }
    }
}