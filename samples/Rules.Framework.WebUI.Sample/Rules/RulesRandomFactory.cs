namespace Rules.Framework.WebUI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Builder.Generic;
    using global::Rules.Framework.WebUI.Sample.Engine;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal class RulesRandomFactory : IRuleSpecificationsRegistrar
    {
        private readonly int finalNumber = 50;
        private readonly int intialNumber = 10;
        private readonly Random random;

        public RulesRandomFactory()
        {
            this.random = new Random();
        }

        public RulesetNames[] Rulesets => new[]
        {
            RulesetNames.TestDateTime,
            RulesetNames.TestDecimal,
            RulesetNames.TestLong,
            RulesetNames.TestBoolean,
            RulesetNames.TestShort,
            RulesetNames.TestNumber,
            RulesetNames.TestString,
            RulesetNames.TestBlob,
        };

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            var currentYear = DateTime.UtcNow.Year;
            var rulesSpecifications = new List<RuleSpecification>();

            foreach (var ruleset in Enum.GetValues(typeof(RulesetNames)).Cast<RulesetNames>())
            {
                for (var i = 1; i < random.Next(intialNumber, finalNumber); i++)
                {
                    var dateBegin = CreateRandomDateBegin(currentYear);

                    Add(CreateMultipleRule(ruleset, i, dateBegin, CreateRandomDateEnd(dateBegin)),
                        RuleAddPriorityOption.ByPriorityNumber(i),
                        rulesSpecifications);
                }

                var deactiveDateBegin = CreateRandomDateBegin(currentYear);

                Add(CreateMultipleRule(ruleset, finalNumber, deactiveDateBegin, CreateRandomDateEnd(deactiveDateBegin), isActive: false),
                        RuleAddPriorityOption.ByPriorityNumber(finalNumber),
                        rulesSpecifications);
            }

            return rulesSpecifications;
        }

        private static RuleBuilderResult<RulesetNames, ConditionNames> CreateMultipleRule(
            RulesetNames ruleset,
            int value,
            DateTime dateBegin,
            DateTime? dateEnd,
            bool isActive = true) => Rule.Create<RulesetNames, ConditionNames>($"Multi rule for test {ruleset} {value}")
                .OnRuleset(ruleset)
                .SetContent(new { Value = value })
                .Since(dateBegin)
                .Until(dateEnd)
                .WithActive(isActive)
                .ApplyWhen(rootCond => rootCond
                    .Or(o => o
                        .Value(ConditionNames.RoyalNumber, Operators.Equal, 7)
                        .Value(ConditionNames.SumAll, Operators.In, new int[] { 9, 8, 6 })
                        .And(a => a
                            .Value(ConditionNames.IsPrimeNumber, Operators.Equal, false)
                            .Value(ConditionNames.SumAll, Operators.StartsWith, "15")
                        )
                        .And(a => a
                            .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, false)
                            .Value(ConditionNames.SumAll, Operators.NotEqual, string.Empty)
                        )
                        .And(a => a
                            .Value(ConditionNames.IsPrimeNumber, Operators.Equal, true)
                            .Value(ConditionNames.SumAll, Operators.StartsWith, "5")
                            .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, false)
                        )))
                .Build();

        private void Add(
            RuleBuilderResult<RulesetNames, ConditionNames> rule,
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