namespace Rules.Framework.InMemory.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Builder.Generic;
    using global::Rules.Framework.InMemory.Sample.Engine;
    using global::Rules.Framework.InMemory.Sample.Enums;

    internal class TestNumberRules : IContentTypes
    {
        public readonly List<RuleSpecification> rulesSpecifications;

        public TestNumberRules()
        {
            this.rulesSpecifications = new List<RuleSpecification>();
        }

        public ContentTypes ContentType => ContentTypes.TestNumber;

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            Add(CreateRuleForCoolNumbers(), RuleAddPriorityOption.ByPriorityNumber(3));
            Add(CreateRuleForSosoNumbers(), RuleAddPriorityOption.ByPriorityNumber(2));
            Add(CreateDefaultRule(), RuleAddPriorityOption.ByPriorityNumber(1));

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

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateDefaultRule() => Rule.New<ContentTypes, ConditionTypes>()
            .WithName("Default rule for test number")
            .WithContent(ContentTypes.TestNumber, ":| default nothing special about this number")
            .WithDateBegin(new DateTime(2019, 01, 01))
            .Build();

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateRuleForCoolNumbers() => Rule.New<ContentTypes, ConditionTypes>()
            .WithName("Rule for cool numbers")
            .WithContent(ContentTypes.TestNumber, ":D this number is so COOL!")
            .WithDateBegin(new DateTime(2019, 01, 01))
            .WithCondition(c => c
            .Or(o => o
                .Value(ConditionTypes.RoyalNumber, Operators.Equal, 7)
                .And(a => a
                    .Value(ConditionTypes.IsPrimeNumber, Operators.Equal, 7)
                    .Value(ConditionTypes.SumAll, Operators.StartsWith, "5"))))
            .Build();

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateRuleForSosoNumbers() => Rule.New<ContentTypes, ConditionTypes>()
            .WithName("Rule for so so numbers")
            .WithContent(ContentTypes.TestNumber, ":) this number is so so")
            .WithDateBegin(new DateTime(2019, 01, 01))
            .WithCondition(c => c
                .Or(o => o
                    .Value(ConditionTypes.CanNumberBeDividedBy3, Operators.Equal, true)
                    .Value(ConditionTypes.IsPrimeNumber, Operators.Equal, false)
                    .Value(ConditionTypes.SumAll, Operators.StartsWith, "9")))
            .Build();
    }
}