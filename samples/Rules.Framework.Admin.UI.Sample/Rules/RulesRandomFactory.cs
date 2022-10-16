namespace Rules.Framework.Admin.UI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Admin.UI.Sample.Engine;
    using global::Rules.Framework.Admin.UI.Sample.Enums;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.Core;

    internal class RulesRandomFactory : IContentTypes
    {
        public readonly List<RuleSpecification> rulesSpecifications;
        private Random gen;

        public RulesRandomFactory()
        {
            this.gen = new Random();
            this.rulesSpecifications = new List<RuleSpecification>();
        }

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            int currentYear = DateTime.UtcNow.Year;

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                var dateBegin = CreateRandomDateBegin(currentYear);

                Add(CreateMultipleRule(ContentTypes.TestDecimal, i, dateBegin, CreateRandomDateEnd(dateBegin)), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                var dateBegin = CreateRandomDateBegin(currentYear);
                Add(CreateMultipleRule(ContentTypes.TestBoolean, i, dateBegin, CreateRandomDateEnd(dateBegin)), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                var dateBegin = CreateRandomDateBegin(currentYear);
                Add(CreateMultipleRule(ContentTypes.TestString, i, dateBegin, CreateRandomDateEnd(dateBegin)), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                var dateBegin = CreateRandomDateBegin(currentYear);
                Add(CreateMultipleRule(ContentTypes.TestShort, i, dateBegin, CreateRandomDateEnd(dateBegin)), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            return this.rulesSpecifications;
        }

        private void Add(
            RuleBuilderResult<ContentTypes, ConditionTypes> rule,
            RuleAddPriorityOption ruleAddPriorityOption) => rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateMultipleRule(ContentTypes contentTypes, int value, DateTime dateBegin,
            DateTime? dateEnd) =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName($"Multi rule for test {contentTypes} {value}")
                             .WithContent(contentTypes, $"multiple value {value}")
                             .WithDatesInterval(dateBegin, dateEnd)
                             .WithCondition(c => c
                                            .AsValued(ConditionTypes.IsPrimeNumber).OfDataType<int>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(value)
                                            .Build())
                            .Build();

        private DateTime CreateRandomDateBegin(int year)
        {
            int months = gen.Next(1, 11);
            year = gen.Next(0, 1) + year;
            return new DateTime(year, 1, 1).AddMonths(months);
        }

        private DateTime? CreateRandomDateEnd(DateTime dateBegin)
        {
            int months = gen.Next(0, 13);
            if (months == 13)
            {
                return null;
            }

            return dateBegin.AddMonths(months).AddDays(1);
        }
    }
}