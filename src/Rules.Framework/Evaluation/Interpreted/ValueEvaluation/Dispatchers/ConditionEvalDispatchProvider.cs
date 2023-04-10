<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/ConditionEvalDispatchProvider.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/ConditionEvalDispatchProvider.cs
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal sealed class ConditionEvalDispatchProvider : IConditionEvalDispatchProvider
    {
        private readonly Dictionary<string, IConditionEvalDispatcher> dispatchers;
        private readonly IMultiplicityEvaluator multiplicityEvaluator;

        public ConditionEvalDispatchProvider(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IMultiplicityEvaluator multiplicityEvaluator,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dispatchers = new Dictionary<string, IConditionEvalDispatcher>(StringComparer.Ordinal)
            {
                { Multiplicities.OneToOne, new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.OneToMany, new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.ManyToOne, new ManyToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.ManyToMany, new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
            };
            this.multiplicityEvaluator = multiplicityEvaluator;
        }

        public IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand)
        {
            string multiplicity = this.multiplicityEvaluator.EvaluateMultiplicity(leftOperand, @operator, rightOperand);

            ThrowIfUnsupportedOperandsAndOperatorCombination($"{multiplicity}-{@operator}");

            return this.dispatchers[multiplicity];
        }

        private static void ThrowIfUnsupportedOperandsAndOperatorCombination(string combination)
        {
            if (!OperatorsMetadata.AllBySupportedCombination.ContainsKey(combination))
            {
                throw new NotSupportedException($"The combination '{combination}' is not supported.");
            }
        }
    }
}