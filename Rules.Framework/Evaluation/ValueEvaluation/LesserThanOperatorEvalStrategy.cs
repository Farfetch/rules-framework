using System;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal class LesserThanOperatorEvalStrategy : IOperatorEvalStrategy
    {
        public bool Eval<T>(T leftOperand, T rightOperand) where T : IComparable<T>
        {
            return leftOperand.CompareTo(rightOperand) < 0;
        }
    }
}