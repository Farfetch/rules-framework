namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
{
    using Rules.Framework;

    internal interface IOperatorEvalStrategyFactory
    {
        IManyToManyOperatorEvalStrategy GetManyToManyOperatorEvalStrategy(Operators @operator);

        IManyToOneOperatorEvalStrategy GetManyToOneOperatorEvalStrategy(Operators @operator);

        IOneToManyOperatorEvalStrategy GetOneToManyOperatorEvalStrategy(Operators @operator);

        IOneToOneOperatorEvalStrategy GetOneToOneOperatorEvalStrategy(Operators @operator);
    }
}