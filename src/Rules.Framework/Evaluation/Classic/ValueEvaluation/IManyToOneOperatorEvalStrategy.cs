namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IManyToOneOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, object rightOperand);
    }
}