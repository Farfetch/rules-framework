namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Rules.Framework.Core;

    internal static class OperatorsMetadata
    {
        private static readonly IDictionary<Operators, OperatorMetadata> allOperatorsMetadata;

        private static readonly IDictionary<string, OperatorMetadata> allOperatorsMetadataBySupportedCombination;

        private static readonly IEnumerable<OperatorMetadata> operatorsMetadata = new[]
        {
            OperatorsMetadata.Equal,
            OperatorsMetadata.NotEqual,
            OperatorsMetadata.GreaterThan,
            OperatorsMetadata.GreaterThanOrEqual,
            OperatorsMetadata.LesserThan,
            OperatorsMetadata.LesserThanOrEqual,
            OperatorsMetadata.Contains,
            OperatorsMetadata.NotContains,
            OperatorsMetadata.In,
            OperatorsMetadata.NotIn,
            OperatorsMetadata.StartsWith,
            OperatorsMetadata.EndsWith,
            OperatorsMetadata.CaseInsensitiveStartsWith,
            OperatorsMetadata.CaseInsensitiveEndsWith,
            OperatorsMetadata.NotStartsWith,
            OperatorsMetadata.NotEndsWith,
        };

        static OperatorsMetadata()
        {
            allOperatorsMetadata = new Dictionary<Operators, OperatorMetadata>();
            var allOperatorsMetadataBySupportedCombinationAux = new Dictionary<string, OperatorMetadata>(StringComparer.Ordinal);

            foreach (var operatorMetadata in operatorsMetadata)
            {
                allOperatorsMetadata.Add(operatorMetadata.Operator, operatorMetadata);

                var supportedCombinations = operatorMetadata.GetAllCombinations();

                foreach (var combination in supportedCombinations)
                {
                    allOperatorsMetadataBySupportedCombinationAux.Add(combination, operatorMetadata);
                }
            }

            allOperatorsMetadataBySupportedCombination
                = new ReadOnlyDictionary<string, OperatorMetadata>(allOperatorsMetadataBySupportedCombinationAux);
        }

        public static IEnumerable<OperatorMetadata> All => allOperatorsMetadata.Values;

        public static IDictionary<Operators, OperatorMetadata> AllByOperator => allOperatorsMetadata;

        public static IDictionary<string, OperatorMetadata> AllBySupportedCombination => allOperatorsMetadataBySupportedCombination;

        public static IEnumerable<string> AllSupportedCombinations => All.SelectMany(x => x.GetAllCombinations());

        public static OperatorMetadata CaseInsensitiveEndsWith => new()
        {
            Operator = Operators.CaseInsensitiveEndsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata CaseInsensitiveStartsWith => new()
        {
            Operator = Operators.CaseInsensitiveStartsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata Contains => new()
        {
            Operator = Operators.Contains,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata EndsWith => new()
        {
            Operator = Operators.EndsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata Equal => new()
        {
            Operator = Operators.Equal,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata GreaterThan => new()
        {
            Operator = Operators.GreaterThan,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata GreaterThanOrEqual => new()
        {
            Operator = Operators.GreaterThanOrEqual,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata In => new()
        {
            Operator = Operators.In,
            SupportedMultiplicities = new[] { Multiplicities.OneToMany },
        };

        public static OperatorMetadata LesserThan => new()
        {
            Operator = Operators.LesserThan,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata LesserThanOrEqual => new()
        {
            Operator = Operators.LesserThanOrEqual,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata NotContains => new()
        {
            Operator = Operators.NotContains,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata NotEndsWith => new()
        {
            Operator = Operators.NotEndsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata NotEqual => new()
        {
            Operator = Operators.NotEqual,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata NotIn => new()
        {
            Operator = Operators.NotIn,
            SupportedMultiplicities = new[] { Multiplicities.OneToMany },
        };

        public static OperatorMetadata NotStartsWith => new()
        {
            Operator = Operators.NotStartsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };

        public static OperatorMetadata StartsWith => new()
        {
            Operator = Operators.StartsWith,
            SupportedMultiplicities = new[] { Multiplicities.OneToOne },
        };
    }
}