namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IOneToManyOperatorEvalStrategy
    {
        bool Eval(object leftOperand, IEnumerable<object> rightOperand);
    }
}