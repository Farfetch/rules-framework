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
            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                Add(CreateMultipleRule(ContentTypes.TestDecimal, i, RandomDayFunc(2019)(), RandomDayFunc(2025)()), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                Add(CreateMultipleRule(ContentTypes.TestBoolean, i, RandomDayFunc(2019)(), RandomDayFunc(2025)()), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                Add(CreateMultipleRule(ContentTypes.TestString, i, RandomDayFunc(2019)(), RandomDayFunc(2025)()), RuleAddPriorityOption.ByPriorityNumber(i));
            }

            for (int i = 1; i < gen.Next(10, 30); i++)
            {
                Add(CreateMultipleRule(ContentTypes.TestShort, i, RandomDayFunc(2019)(), RandomDayFunc(2025)()), RuleAddPriorityOption.ByPriorityNumber(i));
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
            DateTime dateEnd) =>
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

        private Func<DateTime> RandomDayFunc(int year)
        {
            DateTime start = new DateTime(year, 1, 1);

            int range = gen.Next(1, 6);
            return () => start.AddYears(range);
        }
    }
}