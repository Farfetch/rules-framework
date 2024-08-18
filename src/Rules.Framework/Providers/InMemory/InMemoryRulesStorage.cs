namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class InMemoryRulesStorage : IInMemoryRulesStorage
    {
        private readonly ConcurrentDictionary<string, List<RuleDataModel>> rulesByContentType;

        public InMemoryRulesStorage()
        {
            this.rulesByContentType = new ConcurrentDictionary<string, List<RuleDataModel>>(StringComparer.Ordinal);
        }

        public void AddRule(RuleDataModel ruleDataModel)
        {
            var contentTypeRules = GetRulesCollectionByContentType(ruleDataModel.ContentType);

            lock (contentTypeRules)
            {
                if (contentTypeRules.Exists(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' already exists.");
                }

                contentTypeRules.Add(ruleDataModel);
            }
        }

        public void CreateContentType(string contentType)
        {
            _ = this.rulesByContentType.TryAdd(contentType, new List<RuleDataModel>());
        }

        public IReadOnlyCollection<RuleDataModel> GetAllRules()
            => this.rulesByContentType.SelectMany(kvp => kvp.Value).ToList().AsReadOnly();

        public IReadOnlyCollection<string> GetContentTypes()
            => this.rulesByContentType.Keys.ToList().AsReadOnly();

        public IReadOnlyCollection<RuleDataModel> GetRulesBy(string contentType)
        {
            var contentTypeRules = GetRulesCollectionByContentType(contentType);

            return contentTypeRules.AsReadOnly();
        }

        public void UpdateRule(RuleDataModel ruleDataModel)
        {
            var contentTypeRules = GetRulesCollectionByContentType(ruleDataModel.ContentType);

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

        private List<RuleDataModel> GetRulesCollectionByContentType(string contentType) => this.rulesByContentType
                                .GetOrAdd(contentType, (ct) => new List<RuleDataModel>());
    }
}