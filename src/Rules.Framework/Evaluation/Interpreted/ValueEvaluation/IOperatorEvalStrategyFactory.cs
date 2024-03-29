namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using Rules.Framework.Core;

    internal interface IOperatorEvalStrategyFactory
    {
        IManyToManyOperatorEvalStrategy GetManyToManyOperatorEvalStrategy(Operators @operator);

        IManyToOneOperatorEvalStrategy GetManyToOneOperatorEvalStrategy(Operators @operator);

        IOneToManyOperatorEvalStrategy GetOneToManyOperatorEvalStrategy(Operators @operator);

        IOneToOneOperatorEvalStrategy GetOneToOneOperatorEvalStrategy(Operators @operator);
    }
}