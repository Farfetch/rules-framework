namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IManyToManyOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, IEnumerable<object> rightOperand);
    }
}