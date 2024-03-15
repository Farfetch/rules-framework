namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;

    internal sealed class ValueConditionNodeExpressionBuilderProvider : IValueConditionNodeExpressionBuilderProvider
    {
        private readonly Dictionary<string, IValueConditionNodeExpressionBuilder> compilers;

        public ValueConditionNodeExpressionBuilderProvider(
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.compilers = new Dictionary<string, IValueConditionNodeExpressionBuilder>(StringComparer.Ordinal)
            {
                { Multiplicities.OneToOne, new OneToOneValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.OneToMany, new OneToManyValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.ManyToOne, new ManyToOneValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.ManyToMany, new ManyToManyValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
            };
        }

        public IValueConditionNodeExpressionBuilder GetExpressionBuilder(string multiplicity)
        {
            if (this.compilers.TryGetValue(multiplicity, out var compiler))
            {
                return compiler;
            }

            throw new NotSupportedException($"No compiler for multiplicity '{multiplicity}' defined.");
        }
    }
}