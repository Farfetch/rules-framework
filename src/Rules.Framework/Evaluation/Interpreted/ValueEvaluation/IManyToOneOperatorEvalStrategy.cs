<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/IManyToOneOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/IManyToOneOperatorEvalStrategy.cs
{
    using System.Collections.Generic;

    internal interface IManyToOneOperatorEvalStrategy
    {
        bool Eval(IEnumerable<object> leftOperand, object rightOperand);
    }
}