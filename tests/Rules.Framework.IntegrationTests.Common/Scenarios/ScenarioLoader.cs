namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    using Rules.Framework.BenchmarkTests.Tests;

    public static class ScenarioLoader
    {
        public static async Task LoadScenarioAsync<TContentType, TConditionType>(
            IRulesEngine rulesEngine,
            IScenarioData<TContentType, TConditionType> scenarioData)
        {
            var contentTypes = scenarioData.Rules.Select(r => ((Rule)r).ContentType).Distinct().ToArray();
            foreach (var contentType in contentTypes)
            {
                await rulesEngine.CreateContentTypeAsync(contentType).ConfigureAwait(false);
            }

            foreach (var rule in scenarioData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }
        }
    }
}