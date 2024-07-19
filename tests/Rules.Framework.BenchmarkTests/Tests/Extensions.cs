namespace Rules.Framework.BenchmarkTests.Tests
{
    using MongoDB.Driver;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.MongoDb;

    internal static class Extensions
    {
        private const string DatabaseName = "benchmarks-database";

        public static IConfiguredRulesEngineBuilder SetDataSourceForBenchmark(
            this IRulesDataSourceSelector rulesDataSourceSelector,
            string dataSourceName, string benchmarkName)
        {
            return dataSourceName switch
            {
                "in-memory" => rulesDataSourceSelector.SetInMemoryDataSource(),
                "mongo-db" => rulesDataSourceSelector.SetMongoDbDataSource(CreateMongoClient(), new MongoDbProviderSettings
                {
                    DatabaseName = DatabaseName,
                    RulesCollectionName = benchmarkName,
                }),
                _ => throw new NotSupportedException("The specified data source name is not supported"),
            };
        }

        public static async Task TearDownProviderAsync(string dataSourceName, string benchmarkName)
        {
            switch (dataSourceName)
            {
                case "in-memory":
                    break;

                case "mongo-db":
                    var mongoClient = CreateMongoClient();
                    var database = mongoClient.GetDatabase(DatabaseName);
                    await database.DropCollectionAsync(benchmarkName);
                    break;

                default:
                    throw new NotSupportedException("The specified data source name is not supported");
            }
        }

        private static MongoClient CreateMongoClient()
        {
            var settings = MongoClientSettings.FromConnectionString($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");
            return new MongoClient(settings);
        }
    }
}