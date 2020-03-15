namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using MongoDB.Driver;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Serialization;

    public static class MongoDbRulesDataSourceSelectorExtensions
    {
        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetMongoDbDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector,
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings)
        {
            if (rulesDataSourceSelector is null)
            {
                throw new ArgumentNullException(nameof(rulesDataSourceSelector));
            }

            if (mongoClient is null)
            {
                throw new ArgumentNullException(nameof(mongoClient));
            }

            if (mongoDbProviderSettings is null)
            {
                throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            }

            IContentSerializationProvider<TContentType> contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider<TContentType>();
            IRuleFactory<TContentType, TConditionType> ruleFactory = new RuleFactory<TContentType, TConditionType>(contentSerializationProvider);
            MongoDbProviderRulesDataSource<TContentType, TConditionType> mongoDbProviderRulesDataSource
                = new MongoDbProviderRulesDataSource<TContentType, TConditionType>(
                    mongoClient,
                    mongoDbProviderSettings,
                    ruleFactory);

            return rulesDataSourceSelector.SetDataSource(mongoDbProviderRulesDataSource);
        }
    }
}
