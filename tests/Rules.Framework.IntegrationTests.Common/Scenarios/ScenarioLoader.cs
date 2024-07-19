namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    using Rules.Framework.BenchmarkTests.Tests;

    public static class ScenarioLoader
    {
        public static async Task LoadScenarioAsync<TContentType, TConditionType>(
            IRulesEngine rulesEngine,
            IScenarioData<TContentType, TConditionType> scenarioData)
        {
            foreach (var rule in scenarioData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }
        }
    }
}