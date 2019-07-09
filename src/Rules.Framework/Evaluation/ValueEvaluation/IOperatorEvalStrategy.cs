namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal interface IOperatorEvalStrategy
    {
        bool Eval<T>(T leftOperand, T rightOperand)
            where T : IComparable<T>;
    }
}