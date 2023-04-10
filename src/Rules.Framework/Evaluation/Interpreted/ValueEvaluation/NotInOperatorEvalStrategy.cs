<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/NotInOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/NotInOperatorEvalStrategy.cs
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class NotInOperatorEvalStrategy : IOneToManyOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, IEnumerable<object> rightOperand)
            => !rightOperand.Contains(leftOperand);
    }
}