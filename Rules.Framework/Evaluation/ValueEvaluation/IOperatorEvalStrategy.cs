using System;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal interface IOperatorEvalStrategy
    {
        bool Eval<T>(T leftOperand, T rightOperand)
            where T : IComparable<T>;
    }
}