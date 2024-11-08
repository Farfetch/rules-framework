namespace Rules.Framework.InMemory.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Builder.Generic;
    using global::Rules.Framework.InMemory.Sample.Engine;
    using global::Rules.Framework.InMemory.Sample.Enums;

    internal class TestNumberRules : IRuleSpecificationsProvider
    {
        public readonly List<RuleSpecification> rulesSpecifications;

        public TestNumberRules()
        {
            this.rulesSpecifications = new List<RuleSpecification>();
        }

        public RulesetNames[] Rulesets => new[] { RulesetNames.TestNumber, };

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            Add(CreateRuleForCoolNumbers(), RuleAddPriorityOption.ByPriorityNumber(3));
            Add(CreateRuleForSosoNumbers(), RuleAddPriorityOption.ByPriorityNumber(2));
            Add(CreateDefaultRule(), RuleAddPriorityOption.ByPriorityNumber(1));

            return this.rulesSpecifications;
        }

        private void Add(
            RuleBuilderResult<RulesetNames, ConditionNames> rule,
            RuleAddPriorityOption ruleAddPriorityOption) => rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateDefaultRule() => Rule.Create<RulesetNames, ConditionNames>("Default rule for test number")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":| default nothing special about this number")
            .Since(new DateTime(2019, 01, 01))
            .Build();

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateRuleForCoolNumbers() => Rule.Create<RulesetNames, ConditionNames>("Rule for cool numbers")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":D this number is so COOL!")
            .Since(new DateTime(2019, 01, 01))
            .ApplyWhen(c => c
                .Or(o => o
                    .Value(ConditionNames.RoyalNumber, Operators.Equal, 7)
                    .And(a => a
                        .Value(ConditionNames.IsPrimeNumber, Operators.Equal, 7)
                        .Value(ConditionNames.SumAll, Operators.StartsWith, "5"))))
            .Build();

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateRuleForSosoNumbers() => Rule.Create<RulesetNames, ConditionNames>("Rule for so so numbers")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":) this number is so so")
            .Since(new DateTime(2019, 01, 01))
            .ApplyWhen(c => c
                .Or(o => o
                    .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, true)
                    .Value(ConditionNames.IsPrimeNumber, Operators.Equal, false)
                    .Value(ConditionNames.SumAll, Operators.StartsWith, "9")))
            .Build();
    }
}