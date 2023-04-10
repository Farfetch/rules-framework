<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/IConditionEvalDispatcher.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/IConditionEvalDispatcher.cs
{
    using Rules.Framework.Core;

    internal interface IConditionEvalDispatcher
    {
        bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand);
    }
}