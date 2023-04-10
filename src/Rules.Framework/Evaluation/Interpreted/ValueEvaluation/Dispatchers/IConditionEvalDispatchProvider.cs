<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/IConditionEvalDispatchProvider.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/IConditionEvalDispatchProvider.cs
{
    using Rules.Framework.Core;

    internal interface IConditionEvalDispatchProvider
    {
        IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand);
    }
}