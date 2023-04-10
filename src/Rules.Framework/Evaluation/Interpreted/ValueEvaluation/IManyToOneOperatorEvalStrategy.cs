namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IManyToOneOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, object rightOperand);
    }
}