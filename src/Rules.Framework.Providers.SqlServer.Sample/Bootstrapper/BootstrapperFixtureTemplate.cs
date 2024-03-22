namespace Rules.Framework.Providers.SqlServer.Sample.Bootstrapper
{
    public static class BootstrapperFixtureTemplate
    {
        public static async Task InitializeSqlServerAsync()
        {
            var databaseName = "SVC_RULES_FRAMEWORK";
            var connectionString = "Server=sqlserver.docker.internal; User Id=sa; Password=Finance123.; Trusted_Connection=False; Pooling=true; Min Pool Size=1; Max Pool Size=100; MultipleActiveResultSets=true; Application Name=Rules Framework; encrypt=true; trustServerCertificate=true";
            var schemaName = "rulesframework";

            await BootstrapperSqlServerSchema.RecreateSqlSchemaAsync(databaseName, schemaName, connectionString);
        }
    }
}