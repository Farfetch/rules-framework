namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IOneToManyOperatorEvalStrategy
    {
        bool Eval(object leftOperand, IEnumerable<object> rightOperand);
    }
}