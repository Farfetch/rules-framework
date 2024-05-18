namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    public static class ScenarioLoader
    {
        public static async Task LoadScenarioAsync<TContentType, TConditionType>(
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IScenarioData<TContentType, TConditionType> scenarioData)
        {
            foreach (var rule in scenarioData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }
        }
    }
}