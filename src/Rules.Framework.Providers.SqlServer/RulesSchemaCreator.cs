namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    internal class RulesSchemaCreator : IRulesSchemaCreator
    {
        private readonly IEnumerable<Script> schemaScripts;
        private readonly SqlServerDbSettings sqlServerDbSettings;

        public RulesSchemaCreator(SqlServerDbSettings sqlServerDbSettings, IEnumerable<Script> schemaScripts)
        {
            this.sqlServerDbSettings = sqlServerDbSettings ?? throw new ArgumentNullException(nameof(sqlServerDbSettings)); ;
            this.schemaScripts = schemaScripts ?? throw new ArgumentNullException(nameof(schemaScripts)); ;
        }

        public async Task CreateOrUpdateSchemaAsync(string databaseName)
        {
            using (SqlConnection openCon = new SqlConnection(this.sqlServerDbSettings.ConnectionString))
            {
                openCon.Open();

                foreach (var script in this.schemaScripts)
                {
                    string[] batches = script.Value.Split(new string[] { "GO\r\n", "GO\t", "GO\n" }, System.StringSplitOptions.RemoveEmptyEntries);

                    foreach (var batch in batches)
                    {
                        string replacedBatch = batch.Replace("@dbname", databaseName);

                        using (SqlCommand queryCommand = new SqlCommand(replacedBatch))
                        {
                            queryCommand.Connection = openCon;

                            await queryCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }
}