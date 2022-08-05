namespace Rules.Framework.Providers.SqlServer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class SqlServerDbDataProviderFactory
    {
        //public IRetryDurableQueueRepositoryProvider Create(SqlServerDbSettings sqlServerDbSettings)
        //{
        //    Guard.Argument(sqlServerDbSettings)
        //        .NotNull("It is mandatory to config the factory before creating new instances of IRetryQueueDataProvider. Make sure the Config method is executed before the Create method.");

        // var retryQueueItemMessageAdapter = new RetryQueueItemMessageDboFactory();

        // var retryQueueReader = new RetryQueueReader( new RetryQueueAdapter(), new
        // RetryQueueItemAdapter(), new RetryQueueItemMessageAdapter(), new
        // RetryQueueItemMessageHeaderAdapter() );

        //    return new RetryQueueDataProvider(
        //        sqlServerDbSettings,
        //        new ConnectionProvider(),
        //        new RetryQueueItemMessageHeaderRepository(),
        //        new RetryQueueItemMessageRepository(),
        //        new RetryQueueItemRepository(),
        //        new RetryQueueRepository(),
        //        new RetryQueueDboFactory(),
        //        new RetryQueueItemDboFactory(),
        //        retryQueueReader,
        //        retryQueueItemMessageAdapter,
        //        new RetryQueueItemMessageHeaderDboFactory());
        //}

        public IRulesSchemaCreator CreateSchemaCreator(SqlServerDbSettings sqlServerDbSettings)
            => new RulesSchemaCreator(sqlServerDbSettings, this.GetScriptsForSchemaCreation());

        private IEnumerable<Script> GetScriptsForSchemaCreation()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            Script createDatabase;
            Script createTables;
            Script populateTables;

            using (Stream s = thisAssembly.GetManifestResourceStream("Rules.Framework.Providers.SqlServer.Deploy.00-Create_Database.sql"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    createDatabase = new Script(sr.ReadToEnd());
                }
            }

            using (Stream s = thisAssembly.GetManifestResourceStream("Rules.Framework.Providers.SqlServer.Deploy.01-Create_Tables.sql"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    createTables = new Script(sr.ReadToEnd());
                }
            }

            using (Stream s = thisAssembly.GetManifestResourceStream("Rules.Framework.Providers.SqlServer.Deploy.02-Populate_Tables.sql"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    populateTables = new Script(sr.ReadToEnd());
                }
            }

            return new[] { createDatabase, createTables, populateTables };
        }
    }
}