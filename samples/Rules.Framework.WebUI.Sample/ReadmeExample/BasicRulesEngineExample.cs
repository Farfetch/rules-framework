namespace Rules.Framework.WebUI.Sample.ReadmeExample
{
    using System;
    using global::Rules.Framework.WebUI.Sample.Engine;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal class BasicRulesEngineExample
    {
        public BasicRulesEngineExample()
        {
            this.RulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();

            this.CreateRulesets();

            var rules = this.CreateRules();

            this.AddRules(rules);
        }

        public IRulesEngine RulesEngine { get; }

        protected void AddRules(IEnumerable<RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                this.RulesEngine.AddRuleAsync(ruleSpecification.RuleBuilderResult.Rule, ruleSpecification.RuleAddPriorityOption)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected void CreateRulesets()
        {
            foreach (var rulesetName in Enum.GetValues<RulesetNames>())
            {
                this.RulesEngine.CreateRulesetAsync(rulesetName.ToString())
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private IEnumerable<RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>> CreateRules()
        {
            var ruleForPremiumFreeSampleJanuary = Rule.Create<BasicRulesetNames, BasicConditionNames>("Rule for January sample for premium clients.")
                .OnRuleset(BasicRulesetNames.FreeSample)
                .SetContent("SmallPerfumeSample")
                .Since(new DateTime(2023, 01, 01))
                .Until(new DateTime(2023, 02, 01))
                .ApplyWhen(BasicConditionNames.ClientType, Operators.Equal, "Premium")
                .Build();

            var ruleForPremiumFreeSampleApril = Rule.Create<BasicRulesetNames, BasicConditionNames>("Rule for April sample for premium clients.")
                .OnRuleset(BasicRulesetNames.FreeSample)
                .SetContent("ShampooSample")
                .Since(new DateTime(2023, 04, 01))
                .Until(new DateTime(2023, 05, 01))
                .ApplyWhen(BasicConditionNames.ClientType, Operators.Equal, "Premium")
                .Build();

            var ruleForPremiumFreeSampleSeptember = Rule.Create<BasicRulesetNames, BasicConditionNames>("Rule for September sample for premium clients.")
                .OnRuleset(BasicRulesetNames.FreeSample)
                .SetContent("ConditionerSample")
                .Since(new DateTime(2023, 09, 01)).Until(new DateTime(2023, 10, 01))
                .ApplyWhen(BasicConditionNames.ClientType, Operators.Equal, "Premium")
                .Build();

            return new List<RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>>()
            {
                new RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>(
                    ruleForPremiumFreeSampleJanuary, RuleAddPriorityOption.ByPriorityNumber(1)),
                new RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>(
                    ruleForPremiumFreeSampleApril, RuleAddPriorityOption.ByPriorityNumber(2)),
                new RuleSpecificationBase<BasicRulesetNames, BasicConditionNames>(
                    ruleForPremiumFreeSampleSeptember, RuleAddPriorityOption.ByPriorityNumber(3))
            };
        }
    }
}