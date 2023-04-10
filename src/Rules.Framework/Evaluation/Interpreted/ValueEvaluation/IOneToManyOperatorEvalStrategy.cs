namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using System.Collections.Generic;

    internal interface IOneToManyOperatorEvalStrategy
    {
        bool Eval(object leftOperand, IEnumerable<object> rightOperand);
    }
}