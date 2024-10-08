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
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="mongoClient">The mongo client.</param>
        /// <param name="mongoDbProviderSettings">The mongo database provider settings.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// rulesDataSourceSelector or mongoClient or mongoDbProviderSettings
        /// </exception>
        public static IConfiguredRulesEngineBuilder SetMongoDbDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector,
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

            MongoDbRulesDataSourceInitializer.InitializeAsync(mongoClient, mongoDbProviderSettings).GetAwaiter().GetResult();
            IContentSerializationProvider contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider();
            IRuleFactory ruleFactory = new RuleFactory(contentSerializationProvider);
            var mongoDbProviderRulesDataSource
                = new MongoDbProviderRulesDataSource(
                    mongoClient,
                    mongoDbProviderSettings,
                    ruleFactory);

            return rulesDataSourceSelector.SetDataSource(mongoDbProviderRulesDataSource);
        }
    }
}