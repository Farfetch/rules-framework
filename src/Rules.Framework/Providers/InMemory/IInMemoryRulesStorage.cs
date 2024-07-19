namespace Rules.Framework.Providers.InMemory
{
    using System.Collections.Generic;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal interface IInMemoryRulesStorage
    {
        void AddRule(RuleDataModel ruleDataModel);

        void CreateRuleset(string contentType);

        IReadOnlyCollection<RuleDataModel> GetAllRules();

        IReadOnlyCollection<RuleDataModel> GetRulesBy(string ruleset);

        IReadOnlyCollection<RulesetDataModel> GetRulesets();

        void UpdateRule(RuleDataModel ruleDataModel);
    }
}