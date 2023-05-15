namespace Rules.Framework.WebUI.Sample.ReadmeExample
{
    using System;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.Core;
    using global::Rules.Framework.Providers.InMemory;
    using global::Rules.Framework.WebUI.Sample.Engine;

    internal class BasicRulesEngineExample
    {
        public BasicRulesEngineExample()
        {
            this.RulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .WithContentType<BasicContentType>()
                .WithConditionType<BasicConditionType>()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();

            var rules = this.CreateRules();

            this.AddRules(rules);
        }

        public RulesEngine<BasicContentType, BasicConditionType> RulesEngine { get; }

        protected void AddRules(IEnumerable<RuleSpecificationBase<BasicContentType, BasicConditionType>> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                this.RulesEngine.AddRuleAsync(ruleSpecification.RuleBuilderResult.Rule, ruleSpecification.RuleAddPriorityOption)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private IEnumerable<RuleSpecificationBase<BasicContentType, BasicConditionType>> CreateRules()
        {
            var ruleForPremiumFreeSampleJanuary = RuleBuilder
                .NewRule<BasicContentType, BasicConditionType>()
                .WithName("Rule for January sample for premium clients.")
                .WithContent(BasicContentType.FreeSample, "SmallPerfumeSample")
                .WithCondition(BasicConditionType.ClientType, Operators.Equal, "Premium")
                .WithDatesInterval(new DateTime(2023, 01, 01), new DateTime(2023, 02, 01))
                .Build();

            var ruleForPremiumFreeSampleApril = RuleBuilder
                .NewRule<BasicContentType, BasicConditionType>()
                .WithName("Rule for April sample for premium clients.")
                .WithContent(BasicContentType.FreeSample, "ShampooSample")
                .WithCondition(BasicConditionType.ClientType, Operators.Equal, "Premium")
                .WithDatesInterval(new DateTime(2023, 04, 01), new DateTime(2023, 05, 01))
                .Build();

            var ruleForPremiumFreeSampleSeptember = RuleBuilder
                .NewRule<BasicContentType, BasicConditionType>()
                .WithName("Rule for September sample for premium clients.")
                .WithContent(BasicContentType.FreeSample, "ConditionerSample")
                .WithCondition(BasicConditionType.ClientType, Operators.Equal, "Premium")
                .WithDatesInterval(new DateTime(2023, 09, 01), new DateTime(2023, 10, 01))
                .Build();

            return new List<RuleSpecificationBase<BasicContentType, BasicConditionType>>()
            {
                new RuleSpecificationBase<BasicContentType, BasicConditionType>(
                    ruleForPremiumFreeSampleJanuary, RuleAddPriorityOption.ByPriorityNumber(1)),
                new RuleSpecificationBase<BasicContentType, BasicConditionType>(
                    ruleForPremiumFreeSampleApril, RuleAddPriorityOption.ByPriorityNumber(2)),
                new RuleSpecificationBase<BasicContentType, BasicConditionType>(
                    ruleForPremiumFreeSampleSeptember, RuleAddPriorityOption.ByPriorityNumber(3))
            };
        }
    }
}