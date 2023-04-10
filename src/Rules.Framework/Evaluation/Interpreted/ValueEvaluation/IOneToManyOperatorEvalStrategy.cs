<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/IOneToManyOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/IOneToManyOperatorEvalStrategy.cs
{
    using System.Collections.Generic;

    internal interface IOneToManyOperatorEvalStrategy
    {
        bool Eval(object leftOperand, IEnumerable<object> rightOperand);
    }
}