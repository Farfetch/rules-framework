namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class InMemoryRulesStorage : IInMemoryRulesStorage
    {
        private readonly ConcurrentDictionary<string, RulesetDataModel> rulesets;

        public InMemoryRulesStorage()
        {
            this.rulesets = new ConcurrentDictionary<string, RulesetDataModel>(StringComparer.Ordinal);
        }

        public void AddRule(RuleDataModel ruleDataModel)
        {
            var contentTypeRules = this.GetRulesCollectionByRuleset(ruleDataModel.Ruleset);

            lock (contentTypeRules)
            {
                if (contentTypeRules.Exists(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' already exists.");
                }

                contentTypeRules.Add(ruleDataModel);
            }
        }

        public void CreateRuleset(string ruleset)
        {
            _ = this.rulesets.TryAdd(ruleset, new RulesetDataModel
            {
                Creation = DateTime.UtcNow,
                Name = ruleset,
                Rules = new List<RuleDataModel>(),
            });
        }

        public IReadOnlyCollection<RuleDataModel> GetAllRules()
            => this.rulesets.SelectMany(kvp => kvp.Value.Rules).ToList().AsReadOnly();

        public IReadOnlyCollection<RuleDataModel> GetRulesBy(string contentType)
        {
            var rules = this.GetRulesCollectionByRuleset(contentType);

            return rules.AsReadOnly();
        }

        public IReadOnlyCollection<RulesetDataModel> GetRulesets()
            => this.rulesets.Values.ToList().AsReadOnly();

        public void UpdateRule(RuleDataModel ruleDataModel)
        {
            var contentTypeRules = this.GetRulesCollectionByRuleset(ruleDataModel.Ruleset);

            lock (contentTypeRules)
            {
                var existent = contentTypeRules.Find(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal));
                if (existent is null)
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' does not exist, no update can be done.");
                }

                contentTypeRules.Remove(existent);
                contentTypeRules.Add(ruleDataModel);
            }
        }

        private List<RuleDataModel> GetRulesCollectionByRuleset(string ruleset)
        {
            if (this.rulesets.TryGetValue(ruleset, out var rulesetDataModel))
            {
                return rulesetDataModel.Rules;
            }

            throw new InvalidOperationException($"A ruleset with name '{ruleset}' does not exist.");
        }
    }
}