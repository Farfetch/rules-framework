namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    internal interface IOneToOneOperatorEvalStrategy
    {
        bool Eval(object leftOperand, object rightOperand);
    }
}