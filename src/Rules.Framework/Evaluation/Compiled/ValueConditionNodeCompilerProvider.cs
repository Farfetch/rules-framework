namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal sealed class ValueConditionNodeCompilerProvider : IValueConditionNodeCompilerProvider
    {
        private readonly Dictionary<string, IValueConditionNodeCompiler> compilers;

        public ValueConditionNodeCompilerProvider(
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.compilers = new Dictionary<string, IValueConditionNodeCompiler>(StringComparer.Ordinal)
            {
                { Multiplicities.OneToOne, new OneToOneValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider) },
                { Multiplicities.OneToMany, new OneToManyValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider) },
                { Multiplicities.ManyToOne, null },
                { Multiplicities.ManyToMany, null }
            };
        }

        public IValueConditionNodeCompiler GetValueConditionNodeCompiler(string multiplicity)
        {
            if (this.compilers.TryGetValue(multiplicity, out var compiler))
            {
                return compiler;
            }

            throw new NotSupportedException($"No compiler for multiplicity '{multiplicity}' defined.");
        }
    }
}
