<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/IManyToManyOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/IManyToManyOperatorEvalStrategy.cs
{
    using System.Collections.Generic;

    internal interface IManyToManyOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, IEnumerable<object> rightOperand);
    }
}