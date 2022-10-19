namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;

    internal sealed class ConditionExpressionBuilderProvider : IConditionExpressionBuilderProvider
    {
        private readonly IDictionary<string, IConditionExpressionBuilder> conditionExpressionBuilders;

        public ConditionExpressionBuilderProvider()
        {
            this.conditionExpressionBuilders = new Dictionary<string, IConditionExpressionBuilder>(StringComparer.Ordinal)
            {
                { Combine(Operators.Equal, Multiplicities.OneToOne), new EqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotEqual, Multiplicities.OneToOne), new NotEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.GreaterThan, Multiplicities.OneToOne), new GreaterThanOneToOneConditionExpressionBuilder() },
                { Combine(Operators.GreaterThanOrEqual, Multiplicities.OneToOne), new GreaterThanOrEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.LesserThan, Multiplicities.OneToOne), new LesserThanOneToOneConditionExpressionBuilder() },
                { Combine(Operators.LesserThanOrEqual, Multiplicities.OneToOne), new LesserThanOrEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.Contains, Multiplicities.OneToOne), new ContainsOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotContains, Multiplicities.OneToOne), new NotContainsOneToOneConditionExpressionBuilder() },
                { Combine(Operators.In, Multiplicities.OneToMany), new InOneToManyConditionExpressionBuilder() },
                { Combine(Operators.StartsWith, Multiplicities.OneToOne), new StartsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.EndsWith, Multiplicities.OneToOne), new EndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.CaseInsensitiveStartsWith, Multiplicities.OneToOne), new CaseInsensitiveStartsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.CaseInsensitiveEndsWith, Multiplicities.OneToOne), new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotEndsWith, Multiplicities.OneToOne), new NotEndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotStartsWith, Multiplicities.OneToOne), new NotStartsWithOneToOneConditionExpressionBuilder() },
            };
        }

        public IConditionExpressionBuilder GetConditionExpressionBuilderFor(Operators @operator, string multiplicity)
        {
            if (this.conditionExpressionBuilders.TryGetValue(Combine(@operator, multiplicity), out IConditionExpressionBuilder operatorEvalStrategy))
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator compilation is not supported for operator '{@operator}' with '{multiplicity}' multiplicity.");
        }

        private static string Combine(Operators @operator, string multiplicity) => $"{multiplicity}-{@operator}";
    }
}
