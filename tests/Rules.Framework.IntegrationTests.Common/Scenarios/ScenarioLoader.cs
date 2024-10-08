namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    using Rules.Framework.BenchmarkTests.Tests;

    public static class ScenarioLoader
    {
        public static async Task LoadScenarioAsync<TRuleset, TCondition>(
            IRulesEngine rulesEngine,
            IScenarioData<TRuleset, TCondition> scenarioData)
        {
            var rulesets = scenarioData.Rules.Select(r => ((Rule)r).Ruleset).Distinct().ToArray();
            foreach (var ruleset in rulesets)
            {
                await rulesEngine.CreateRulesetAsync(ruleset).ConfigureAwait(false);
            }

            foreach (var rule in scenarioData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }
        }
    }
}