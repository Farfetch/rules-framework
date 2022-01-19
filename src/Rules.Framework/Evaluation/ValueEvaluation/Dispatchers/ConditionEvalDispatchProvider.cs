namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using Rules.Framework.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConditionEvalDispatchProvider : IConditionEvalDispatchProvider
    {
        private const string ManyToMany = "many-to-many";
        private const string ManyToOne = "many-to-one";
        private const string OneToMany = "one-to-many";
        private const string OneToOne = "one-to-one";
        private readonly Dictionary<string, IConditionEvalDispatcher> dispatchers;
        private readonly string[] supportedCombinations;

        public ConditionEvalDispatchProvider(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dispatchers = new Dictionary<string, IConditionEvalDispatcher>
            {
                { OneToOne, new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { OneToMany, new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { ManyToOne, new ManyToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { ManyToMany, new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) }
            };
            this.supportedCombinations = new[]
            {
                $"{OneToOne}-{Operators.Equal}",
                $"{OneToOne}-{Operators.NotEqual}",
                $"{OneToOne}-{Operators.GreaterThan}",
                $"{OneToOne}-{Operators.GreaterThanOrEqual}",
                $"{OneToOne}-{Operators.LesserThan}",
                $"{OneToOne}-{Operators.LesserThanOrEqual}",
                $"{OneToOne}-{Operators.Contains}",
                $"{OneToMany}-{Operators.In}",
                $"{OneToOne}-{Operators.StartsWith}",
                $"{OneToOne}-{Operators.EndsWith}",
            };
        }

        public IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand)
        {
            string combination = leftOperand switch
            {
                IEnumerable _ when !(leftOperand is string) && rightOperand is IEnumerable && !(rightOperand is string) => ManyToMany,
                IEnumerable _ when !(leftOperand is string) => ManyToOne,
                object _ when rightOperand is IEnumerable && !(rightOperand is string) => OneToMany,
                object _ => OneToOne,
                null when OperatorSupportsOneMultiplicityLeftOperand(@operator) && rightOperand is IEnumerable && !(rightOperand is string) => OneToMany,
                null when OperatorSupportsOneMultiplicityLeftOperand(@operator) => OneToOne,
                _ => throw new NotSupportedException()
            };

            this.ThrowIfUnsupportedOperandsAndOperatorCombination($"{combination}-{@operator}");

            return this.dispatchers[combination];
        }

        private bool OperatorSupportsOneMultiplicityLeftOperand(Operators @operator)
        {
            return this.supportedCombinations.Where(x => x.Contains(@operator.ToString())).Any(x => x.Contains("one-to"));
        }

        private void ThrowIfUnsupportedOperandsAndOperatorCombination(string combination)
        {
            if (!this.supportedCombinations.Contains(combination))
            {
                throw new NotSupportedException($"The combination '{combination}' is not supported.");
            }
        }
    }
}