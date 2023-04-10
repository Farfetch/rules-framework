<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/InOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/InOperatorEvalStrategy.cs
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class InOperatorEvalStrategy : IOneToManyOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, IEnumerable<object> rightOperand)
            => rightOperand.Contains(leftOperand);
    }
}