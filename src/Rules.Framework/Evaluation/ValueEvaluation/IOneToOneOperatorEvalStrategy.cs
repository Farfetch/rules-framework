namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal interface IOneToOneOperatorEvalStrategy
    {
        bool Eval(object leftOperand, object rightOperand);
    }
}