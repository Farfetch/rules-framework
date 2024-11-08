namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Rules.Framework;

    internal interface IConditionEvalDispatchProvider
    {
        IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand);
    }
}