namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    internal class RulesSchemaCreator : IRulesSchemaCreator
    {
        private readonly IEnumerable<Script> schemaScripts;
        private readonly SqlServerDbSettings sqlServerDbSettings;

        public RulesSchemaCreator(SqlServerDbSettings sqlServerDbSettings, IEnumerable<Script> schemaScripts)
        {
            this.sqlServerDbSettings = sqlServerDbSettings ?? throw new ArgumentNullException(nameof(sqlServerDbSettings)); ;
            this.schemaScripts = schemaScripts ?? throw new ArgumentNullException(nameof(schemaScripts));
        }

        public async Task CreateOrUpdateSchemaAsync(string databaseName, string schemaName)
        {
            using (SqlConnection openCon = new SqlConnection(this.sqlServerDbSettings.ConnectionString))
            {
                openCon.Open();

                foreach (var script in this.schemaScripts)
                {
                    string[] batches = script.Value.Split(new string[] { "GO\r\n", "GO\t", "GO\n" }, System.StringSplitOptions.RemoveEmptyEntries);

                    foreach (var batch in batches)
                    {
                        var replacedBatch = batch
                                            .Replace("@dbname", databaseName)
                                            .Replace("@schemaname", schemaName);

                        using (SqlCommand queryCommand = new SqlCommand(replacedBatch))
                        {
                            queryCommand.Connection = openCon;

                            await queryCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public async Task CreateOrUpdateSchemaAsync(string databaseName)
        {
            await CreateOrUpdateSchemaAsync(databaseName, "dbo")
                    .ConfigureAwait(false);
        }
    }
}