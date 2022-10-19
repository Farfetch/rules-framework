namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
{
    using Rules.Framework.Core;

    internal interface IConditionEvalDispatcher
    {
        bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand);
    }
}