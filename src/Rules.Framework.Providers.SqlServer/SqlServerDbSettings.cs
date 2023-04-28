namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
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