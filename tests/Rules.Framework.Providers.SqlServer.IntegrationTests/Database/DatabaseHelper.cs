namespace Rules.Framework.Providers.SqlServer.IntegrationTests.Database
{
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    internal class DatabaseHelper
    {
        public async Task ExecuteScriptAsync(SqlServerDbSettings sqlServerDbSettings, string resourceName, string schemaName = null)
        {
            var script = LoadResource(resourceName);

            using (SqlConnection openCon = new SqlConnection(sqlServerDbSettings.ConnectionString))
            {
                openCon.Open();

                string[] batches = script.Value.Split(new string[] { "GO\r\n", "GO\t", "GO\n" }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var batch in batches)
                {
                    var replacedBatch = batch
                                        .Replace("@dbname", sqlServerDbSettings.DatabaseName)
                                        .Replace("@schemaname", schemaName ?? "dbo");

                    using (SqlCommand queryCommand = new SqlCommand(replacedBatch))
                    {
                        queryCommand.Connection = openCon;

                        await queryCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        internal Script LoadResource(string resourceName)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    return new Script(sr.ReadToEnd());
                }
            }
        }
    }
}