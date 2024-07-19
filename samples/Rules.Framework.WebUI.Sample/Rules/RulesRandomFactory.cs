namespace Rules.Framework.WebUI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Builder.Generic;
    using global::Rules.Framework.WebUI.Sample.Engine;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal class RulesRandomFactory : IContentTypes
    {
        private readonly int finalNumber = 50;
        private readonly int intialNumber = 10;
        private readonly Random random;

        public RulesRandomFactory()
        {
            this.random = new Random();
        }

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            var currentYear = DateTime.UtcNow.Year;
            var rulesSpecifications = new List<RuleSpecification>();

            foreach (var contentType in Enum.GetValues(typeof(ContentTypes)))
            {
                for (var i = 1; i < random.Next(intialNumber, finalNumber); i++)
                {
                    var dateBegin = CreateRandomDateBegin(currentYear);

                    Add(CreateMultipleRule((ContentTypes)contentType, i, dateBegin, CreateRandomDateEnd(dateBegin)),
                        RuleAddPriorityOption.ByPriorityNumber(i),
                        rulesSpecifications);
                }

                var deactiveDateBegin = CreateRandomDateBegin(currentYear);

                Add(CreateMultipleRule((ContentTypes)contentType, finalNumber, deactiveDateBegin, CreateRandomDateEnd(deactiveDateBegin), isActive: false),
                        RuleAddPriorityOption.ByPriorityNumber(finalNumber),
                        rulesSpecifications);
            }

            return rulesSpecifications;
        }

        private static RuleBuilderResult<ContentTypes, ConditionTypes> CreateMultipleRule(
            ContentTypes contentTypes,
            int value,
            DateTime dateBegin,
            DateTime? dateEnd,
            bool isActive = true) => Rule.New<ContentTypes, ConditionTypes>()
                .WithName($"Multi rule for test {contentTypes} {value}")
                .WithContent(contentTypes, new { Value = value })
                .WithDatesInterval(dateBegin, dateEnd)
                .WithActive(isActive)
                .WithCondition(rootCond => rootCond
                    .Or(o => o
                        .Value(ConditionTypes.RoyalNumber, Operators.Equal, 7)
                        .Value(ConditionTypes.SumAll, Operators.In, new int[] { 9, 8, 6 })
                        .And(a => a
                            .Value(ConditionTypes.IsPrimeNumber, Operators.Equal, false)
                            .Value(ConditionTypes.SumAll, Operators.StartsWith, "15")
                        )
                        .And(a => a
                            .Value(ConditionTypes.CanNumberBeDividedBy3, Operators.Equal, false)
                            .Value(ConditionTypes.SumAll, Operators.NotEqual, string.Empty)
                        )
                        .And(a => a
                            .Value(ConditionTypes.IsPrimeNumber, Operators.Equal, true)
                            .Value(ConditionTypes.SumAll, Operators.StartsWith, "5")
                            .Value(ConditionTypes.CanNumberBeDividedBy3, Operators.Equal, false)
                        )))
                .Build();

        private void Add(
            RuleBuilderResult<ContentTypes, ConditionTypes> rule,
            RuleAddPriorityOption ruleAddPriorityOption, List<RuleSpecification> rulesSpecifications)
            => rulesSpecifications.Add(new RuleSpecification(rule, ruleAddPriorityOption));

        private DateTime CreateRandomDateBegin(int year)
        {
            var months = random.Next(1, 11);
            year = random.Next(0, 1) + year;
            return new DateTime(year, 1, 1).AddMonths(months);
        }

        private DateTime? CreateRandomDateEnd(DateTime dateBegin)
        {
            var months = random.Next(0, 13);
            if (months == 13)
            {
                return null;
            }

            return dateBegin.AddMonths(months).AddDays(1);
        }
    }
}