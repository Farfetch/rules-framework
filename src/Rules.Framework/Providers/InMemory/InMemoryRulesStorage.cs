namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class InMemoryRulesStorage<TContentType, TConditionType> : IInMemoryRulesStorage<TContentType, TConditionType>
    {
        private readonly ConcurrentDictionary<TContentType, List<RuleDataModel<TContentType, TConditionType>>> rulesByContentType;

        public InMemoryRulesStorage()
        {
            this.rulesByContentType = new ConcurrentDictionary<TContentType, List<RuleDataModel<TContentType, TConditionType>>>();
        }

        public void AddRule(RuleDataModel<TContentType, TConditionType> ruleDataModel)
        {
            List<RuleDataModel<TContentType, TConditionType>> contentTypeRules = GetRulesCollectionByContentType(ruleDataModel.ContentType);

            lock (contentTypeRules)
            {
                if (contentTypeRules.Exists(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' already exists.");
                }

                contentTypeRules.Add(ruleDataModel);
            }
        }

        public IReadOnlyCollection<RuleDataModel<TContentType, TConditionType>> GetAllRules()
            => this.rulesByContentType.SelectMany(kvp => kvp.Value).ToList().AsReadOnly();

        public IReadOnlyCollection<RuleDataModel<TContentType, TConditionType>> GetRulesBy(TContentType contentType)
        {
            List<RuleDataModel<TContentType, TConditionType>> contentTypeRules = GetRulesCollectionByContentType(contentType);

            return contentTypeRules.AsReadOnly();
        }

        public void UpdateRule(RuleDataModel<TContentType, TConditionType> ruleDataModel)
        {
            List<RuleDataModel<TContentType, TConditionType>> contentTypeRules = GetRulesCollectionByContentType(ruleDataModel.ContentType);

            lock (contentTypeRules)
            {
                RuleDataModel<TContentType, TConditionType> existent = contentTypeRules.Find(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal));
                if (existent is null)
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' does not exist, no update can be done.");
                }

                contentTypeRules.Remove(existent);
                contentTypeRules.Add(ruleDataModel);
            }
        }

        private List<RuleDataModel<TContentType, TConditionType>> GetRulesCollectionByContentType(TContentType contentType) => this.rulesByContentType
                                .GetOrAdd(contentType, (ct) => new List<RuleDataModel<TContentType, TConditionType>>());
    }
}