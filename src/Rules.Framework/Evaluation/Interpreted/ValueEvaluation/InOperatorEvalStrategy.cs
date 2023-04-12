namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class InOperatorEvalStrategy : IOneToManyOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, IEnumerable<object> rightOperand)
            => rightOperand.Contains(leftOperand);
    }
}