namespace Rules.Framework.Providers.InMemory
{
    using System.Collections.Generic;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal interface IInMemoryRulesStorage
    {
        void AddRule(RuleDataModel ruleDataModel);

        void CreateContentType(string contentType);

        IReadOnlyCollection<RuleDataModel> GetAllRules();

        IReadOnlyCollection<string> GetContentTypes();

        IReadOnlyCollection<RuleDataModel> GetRulesBy(string contentType);

        void UpdateRule(RuleDataModel ruleDataModel);
    }
}