namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using Rules.Framework.Core;

    internal interface IOperatorEvalStrategyFactory
    {
        IOperatorEvalStrategy GetOperatorEvalStrategy(Operators @operator);
    }
}