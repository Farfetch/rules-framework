namespace Rules.Framework.BenchmarkTests
{
    using System;
    using DotEnv.Core;

    internal static class SettingsProvider
    {
        private static IEnvironmentVariablesProvider? environmentVariablesProvider;

        public static string GetMongoDbHost()
        {
            if (environmentVariablesProvider is null)
            {
                environmentVariablesProvider = new EnvLoader().Load();
            }

            string? mongoDbHost = Environment.GetEnvironmentVariable("MONGO_DB_HOST") ?? environmentVariablesProvider["MONGO_DB_HOST"];

            return mongoDbHost ?? "localhost";
        }
    }
}