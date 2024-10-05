namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Providers.MongoDb.DataModel;

    internal static class MongoDbRulesDataSourceInitializer
    {
        private static bool isInitialized;

        public static async Task InitializeAsync(IMongoClient mongoClient, MongoDbProviderSettings mongoDbProviderSettings)
        {
            if (isInitialized)
            {
                return;
            }

            if (mongoClient is null)
            {
                throw new ArgumentNullException(nameof(mongoClient));
            }

            if (mongoDbProviderSettings is null)
            {
                throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            }

            var mongoDatabase = mongoClient.GetDatabase(mongoDbProviderSettings.DatabaseName, new MongoDatabaseSettings
            {
                ReadConcern = ReadConcern.Available,
                ReadPreference = ReadPreference.SecondaryPreferred,
                WriteConcern = WriteConcern.Acknowledged,
            });

            await CreateCollectionsIfNotExists(mongoDatabase, mongoDbProviderSettings).ConfigureAwait(false);
            await CreateIndexesIfNotExists(mongoDatabase, mongoDbProviderSettings).ConfigureAwait(false);

            // Mark as initialized as this initialization code is never run anymore throughout the
            // app lifecycle.
            isInitialized = true;
        }

        private static async Task CreateCollectionsIfNotExists(IMongoDatabase mongoDatabase, MongoDbProviderSettings mongoDbProviderSettings)
        {
            var collectionsCursor = await mongoDatabase.ListCollectionNamesAsync().ConfigureAwait(false);
            var collections = await collectionsCursor.ToListAsync().ConfigureAwait(false);

            if (!collections.Contains(mongoDbProviderSettings.RulesetsCollectionName))
            {
                await mongoDatabase.CreateCollectionAsync(mongoDbProviderSettings.RulesetsCollectionName).ConfigureAwait(false);
            }

            if (!collections.Contains(mongoDbProviderSettings.RulesCollectionName))
            {
                await mongoDatabase.CreateCollectionAsync(mongoDbProviderSettings.RulesCollectionName).ConfigureAwait(false);
            }
        }

        private static async Task CreateIndexesIfNotExists(IMongoDatabase mongoDatabase, MongoDbProviderSettings mongoDbProviderSettings)
        {
            var rulesetsCollection = mongoDatabase.GetCollection<RulesetDataModel>(mongoDbProviderSettings.RulesetsCollectionName);
            var rulesCollection = mongoDatabase.GetCollection<RuleDataModel>(mongoDbProviderSettings.RulesCollectionName);

            var rulesetsByNameIndexName = $"ix_{mongoDbProviderSettings.RulesetsCollectionName.ToLower(CultureInfo.InvariantCulture)}_rulesets_by_name";
            var rulesetsByNameIndexKeys = Builders<RulesetDataModel>.IndexKeys
                .Ascending(x => x.Name);
            await CreateIndexOnBackgroundAsync(rulesetsCollection, rulesetsByNameIndexName, rulesetsByNameIndexKeys).ConfigureAwait(false);

            var rulesByRulesetAndDatesIndexName = $"ix_{mongoDbProviderSettings.RulesCollectionName.ToLower(CultureInfo.InvariantCulture)}_rules_by_ruleset_and_dates";
            var rulesByRulesetAndDatesIndexKeys = Builders<RuleDataModel>.IndexKeys
                .Ascending(x => x.Ruleset).Ascending(x => x.DateBegin).Ascending(x => x.DateEnd);
            await CreateIndexOnBackgroundAsync(rulesCollection, rulesByRulesetAndDatesIndexName, rulesByRulesetAndDatesIndexKeys).ConfigureAwait(false);

            var rulesByRulesetNamePriorityIndexName = $"ix_{mongoDbProviderSettings.RulesCollectionName.ToLower(CultureInfo.InvariantCulture)}_rules_by_ruleset_name_priority";
            var rulesByRulesetNamePriorityIndexKeys = Builders<RuleDataModel>.IndexKeys
                .Ascending(x => x.Ruleset).Ascending(x => x.Name).Ascending(x => x.Priority);
            await CreateIndexOnBackgroundAsync(rulesCollection, rulesByRulesetNamePriorityIndexName, rulesByRulesetNamePriorityIndexKeys).ConfigureAwait(false);
        }

        private static async Task CreateIndexOnBackgroundAsync<T>(IMongoCollection<T> mongoCollection, string indexName, IndexKeysDefinition<T> indexKeysDefinition)
        {
            var createIndexOptions = new CreateIndexOptions
            {
                Background = true,
                Name = indexName,
            };
            var createIndexModel = new CreateIndexModel<T>(indexKeysDefinition, createIndexOptions);
            _ = await mongoCollection.Indexes.CreateOneAsync(createIndexModel).ConfigureAwait(false);
        }
    }
}