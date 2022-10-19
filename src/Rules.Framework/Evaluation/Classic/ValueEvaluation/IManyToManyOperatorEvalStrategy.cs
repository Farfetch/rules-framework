namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IManyToManyOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, IEnumerable<object> rightOperand);
    }
}