<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/IOperatorEvalStrategyFactory.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/IOperatorEvalStrategyFactory.cs
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