namespace Rules.Framework.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Rules.Framework.WebUI.Sample.Exceptions;

    internal class RulesBuilder
    {
        private readonly IEnumerable<IRuleSpecificationsProvider> ruleSpecificationsProviders;

        public RulesBuilder(IEnumerable<IRuleSpecificationsProvider> ruleSpecificationsProviders) => this.ruleSpecificationsProviders = ruleSpecificationsProviders;

        public async Task BuildAsync(IRulesEngine rulesEngine)
        {
            foreach (var ruleSpecificationsProvider in ruleSpecificationsProviders)
            {
                foreach (var ruleset in ruleSpecificationsProvider.Rulesets)
                {
                    await rulesEngine.CreateRulesetAsync(ruleset.ToString());
                }

                var rulesSpecifications = ruleSpecificationsProvider.GetRulesSpecifications();

                foreach (var ruleSpecification in rulesSpecifications)
                {
                    if (!ruleSpecification.RuleBuilderResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error getting rules specifications", ruleSpecification.RuleBuilderResult.Errors);
                    }

                    var ruleOperationResult = await rulesEngine
                        .AddRuleAsync(
                            ruleSpecification.RuleBuilderResult.Rule,
                            ruleSpecification.RuleAddPriorityOption
                        );

                    if (!ruleOperationResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error adding rules to engine", ruleOperationResult.Errors);
                    }
                }
            }
        }
    }
}