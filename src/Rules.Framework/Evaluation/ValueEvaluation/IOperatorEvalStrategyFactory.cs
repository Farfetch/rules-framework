using Rules.Framework.Core;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal interface IOperatorEvalStrategyFactory
    {
        IOperatorEvalStrategy GetOperatorEvalStrategy(Operators @operator);
    }
}