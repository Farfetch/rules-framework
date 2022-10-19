namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
{
    internal interface IOneToOneOperatorEvalStrategy
    {
        bool Eval(object leftOperand, object rightOperand);
    }
}