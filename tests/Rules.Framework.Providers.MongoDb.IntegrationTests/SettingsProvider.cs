namespace Rules.Framework.Providers.MongoDb.IntegrationTests
{
    using System;

    internal static class SettingsProvider
    {
        public static string GetMongoDbHost()
        {
            string? mongoDbHost = Environment.GetEnvironmentVariable("MONGO_DB_HOST");

            return mongoDbHost ?? "localhost";
        }
    }
}