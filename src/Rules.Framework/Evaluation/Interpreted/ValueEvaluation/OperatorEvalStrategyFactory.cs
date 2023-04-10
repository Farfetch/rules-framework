<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/OperatorEvalStrategyFactory.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/OperatorEvalStrategyFactory.cs
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal sealed class OperatorEvalStrategyFactory : IOperatorEvalStrategyFactory
    {
        private readonly IDictionary<Operators, object> strategies;

        public OperatorEvalStrategyFactory()
        {
            this.strategies = new Dictionary<Operators, object>
            {
                { Operators.Equal, new EqualOperatorEvalStrategy() },
                { Operators.NotEqual, new NotEqualOperatorEvalStrategy() },
                { Operators.GreaterThan, new GreaterThanOperatorEvalStrategy() },
                { Operators.GreaterThanOrEqual, new GreaterThanOrEqualOperatorEvalStrategy() },
                { Operators.LesserThan, new LesserThanOperatorEvalStrategy() },
                { Operators.LesserThanOrEqual, new LesserThanOrEqualOperatorEvalStrategy() },
                { Operators.Contains, new ContainsOperatorEvalStrategy() },
                { Operators.NotContains, new NotContainsOperatorEvalStrategy() },
                { Operators.In, new InOperatorEvalStrategy() },
                { Operators.NotIn, new NotInOperatorEvalStrategy() },
                { Operators.StartsWith, new StartsWithOperatorEvalStrategy() },
                { Operators.EndsWith, new EndsWithOperatorEvalStrategy() },
                { Operators.CaseInsensitiveStartsWith, new CaseInsensitiveStartsWithOperatorEvalStrategy() },
                { Operators.CaseInsensitiveEndsWith, new CaseInsensitiveEndsWithOperatorEvalStrategy() },
                { Operators.NotStartsWith, new NotStartsWithOperatorEvalStrategy() },
                { Operators.NotEndsWith, new NotEndsWithOperatorEvalStrategy() }
            };
        }

        public IManyToManyOperatorEvalStrategy GetManyToManyOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out object operatorEvalStrategy) && operatorEvalStrategy is IManyToManyOperatorEvalStrategy casted)
            {
                return casted;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IManyToManyOperatorEvalStrategy)}.");
        }

        public IManyToOneOperatorEvalStrategy GetManyToOneOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out object operatorEvalStrategy) && operatorEvalStrategy is IManyToOneOperatorEvalStrategy casted)
            {
                return casted;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IManyToOneOperatorEvalStrategy)}.");
        }

        public IOneToManyOperatorEvalStrategy GetOneToManyOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out object operatorEvalStrategy) && operatorEvalStrategy is IOneToManyOperatorEvalStrategy casted)
            {
                return casted;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IOneToManyOperatorEvalStrategy)}.");
        }

        public IOneToOneOperatorEvalStrategy GetOneToOneOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out object operatorEvalStrategy) && operatorEvalStrategy is IOneToOneOperatorEvalStrategy casted)
            {
                return casted;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IOneToOneOperatorEvalStrategy)}.");
        }
    }
}