namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using Rules.Framework.Core;

    internal interface IConditionEvalDispatchProvider
    {
        IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand);
    }
}