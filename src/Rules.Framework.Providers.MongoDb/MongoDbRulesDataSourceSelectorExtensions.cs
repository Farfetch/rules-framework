namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using MongoDB.Driver;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Serialization;

    /// <summary>
    /// Rules data source selector extensions from Mongo DB provider.
    /// </summary>
    public static class MongoDbRulesDataSourceSelectorExtensions
    {
        /// <summary>
        /// Sets the rules engine data source from a Mongo DB database.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="mongoClient">The mongo client.</param>
        /// <param name="mongoDbProviderSettings">The mongo database provider settings.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// rulesDataSourceSelector
        /// or
        /// mongoClient
        /// or
        /// mongoDbProviderSettings
        /// </exception>
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