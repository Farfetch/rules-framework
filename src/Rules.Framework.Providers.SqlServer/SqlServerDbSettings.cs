namespace Rules.Framework.Providers.SqlServer
{
    using System;

    public class SqlServerDbSettings
    {
        public SqlServerDbSettings(string connectionString, string databaseName)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string ConnectionString { get; }

        public string DatabaseName { get; }
    }
}