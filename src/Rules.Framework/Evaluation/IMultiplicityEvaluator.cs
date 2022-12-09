namespace Rules.Framework.Evaluation
{
    using Rules.Framework.Core;

    internal interface IMultiplicityEvaluator
    {
        string EvaluateMultiplicity(object leftOperand, Operators @operator, object rightOperand);
    }
}