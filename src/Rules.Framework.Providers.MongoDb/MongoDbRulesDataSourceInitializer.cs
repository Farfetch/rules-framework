namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Providers.MongoDb.DataModel;

    internal static class MongoDbRulesDataSourceInitializer
    {
        private static bool isInitialized = false;

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

            // Creates rules collection if it does not exist.
            var collectionsCursor = await mongoDatabase.ListCollectionNamesAsync().ConfigureAwait(false);
            var collections = await collectionsCursor.ToListAsync().ConfigureAwait(false);
            if (!collections.Contains(mongoDbProviderSettings.RulesCollectionName))
            {
                await mongoDatabase.CreateCollectionAsync(mongoDbProviderSettings.RulesCollectionName).ConfigureAwait(false);
            }

            var rulesCollection = mongoDatabase.GetCollection<RuleDataModel>(mongoDbProviderSettings.RulesCollectionName);

            // Create indexes if they do not exist.
            var getRulesIndex = $"ix_{mongoDbProviderSettings.RulesCollectionName.ToLower(CultureInfo.InvariantCulture)}_get_rules";
            var getRulesIndexKeysDefinition = Builders<RuleDataModel>.IndexKeys
                .Ascending("ContentType").Ascending("DateBegin").Ascending("DateEnd");
            await CreateIndexOnBackgroundAsync(rulesCollection, getRulesIndex, getRulesIndexKeysDefinition).ConfigureAwait(false);
            var getRulesByIndex = $"ix_{mongoDbProviderSettings.RulesCollectionName.ToLower(CultureInfo.InvariantCulture)}_get_rules_by";
            var getRulesByIndexKeysDefinition = Builders<RuleDataModel>.IndexKeys
                .Ascending("ContentType").Ascending("Name").Ascending("Priority");
            await CreateIndexOnBackgroundAsync(rulesCollection, getRulesByIndex, getRulesByIndexKeysDefinition).ConfigureAwait(false);

            // Mark as initialized as this initialization code is never run anymore throughout the
            // app lifecycle.
            isInitialized = true;
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