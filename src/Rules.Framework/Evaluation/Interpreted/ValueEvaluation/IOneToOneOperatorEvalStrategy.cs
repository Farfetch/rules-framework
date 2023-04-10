<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/IOneToOneOperatorEvalStrategy.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/IOneToOneOperatorEvalStrategy.cs
{
    internal interface IOneToOneOperatorEvalStrategy
    {
        bool Eval(object leftOperand, object rightOperand);
    }
}