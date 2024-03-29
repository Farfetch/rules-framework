namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class NotInOperatorEvalStrategy : IOneToManyOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, IEnumerable<object> rightOperand)
            => !rightOperand.Contains(leftOperand);
    }
}