namespace Rules.Framework.Admin.UI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Admin.UI.Sample.Enums;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.Core;
    using global::Rules.Framework.UI.Sample;

    internal class RulesRandomFactory : IContentTypes
    {
        public readonly List<RuleSpecification> rulesSpecifications;
        private readonly int finalNumber = 50;
        private readonly int intialNumber = 10;
        private readonly Random random;

        public RulesRandomFactory()
        {
            this.random = new Random();
            this.rulesSpecifications = new List<RuleSpecification>();
        }

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            int currentYear = DateTime.UtcNow.Year;

            foreach (int contentType in Enum.GetValues(typeof(ContentTypes)))
            {
                for (int i = 1; i < random.Next(intialNumber, finalNumber); i++)
                {
                    var dateBegin = CreateRandomDateBegin(currentYear);

                    Add(CreateMultipleRule((ContentTypes)contentType, i, dateBegin, CreateRandomDateEnd(dateBegin)), RuleAddPriorityOption.ByPriorityNumber(i));
                }
            }

            return this.rulesSpecifications;
        }

        private static RuleBuilderResult<ContentTypes, ConditionTypes> CreateMultipleRule(ContentTypes contentTypes, int value, DateTime dateBegin,
            DateTime? dateEnd) =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName($"Multi rule for test {contentTypes} {value}")
                             .WithContent(contentTypes, $"Value {contentTypes} {value}")
                             .WithDatesInterval(dateBegin, dateEnd)
                             .WithCondition(cnb => cnb.AsComposed()
                                    .WithLogicalOperator(LogicalOperators.Or)
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.RoyalNumber).OfDataType<int>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(7)
                                            .Build())
                                        .AddCondition(condition => condition.AsComposed()
                                            .WithLogicalOperator(LogicalOperators.And)
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.IsPrimeNumber).OfDataType<bool>()
                                                .WithComparisonOperator(Operators.Equal)
                                                .SetOperand(true)
                                                .Build())
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.SumAll).OfDataType<string>()
                                                .WithComparisonOperator(Operators.StartsWith)
                                                .SetOperand("5")
                                                .Build())
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.CanNumberBeDividedBy3).OfDataType<bool>()
                                                .WithComparisonOperator(Operators.Equal)
                                                .SetOperand(false)
                                                .Build())
                                            .Build())
                                        .Build())
                            .Build();

        private void Add(
                    RuleBuilderResult<ContentTypes, ConditionTypes> rule,
            RuleAddPriorityOption ruleAddPriorityOption) => rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private DateTime CreateRandomDateBegin(int year)
        {
            int months = random.Next(1, 11);
            year = random.Next(0, 1) + year;
            return new DateTime(year, 1, 1).AddMonths(months);
        }

        private DateTime? CreateRandomDateEnd(DateTime dateBegin)
        {
            int months = random.Next(0, 13);
            if (months == 13)
            {
                return null;
            }

            return dateBegin.AddMonths(months).AddDays(1);
        }
    }
}