namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Rules.Framework;

    internal interface IConditionEvalDispatcher
    {
        bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand);
    }
}