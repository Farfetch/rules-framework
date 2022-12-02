namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.SqlServer.Serialization;
    using Rules.Framework.SqlServer.Models;

    /// <summary>
    /// Rules data source selector extensions from SQL Server database provider.
    /// </summary>
    public static class SqlServerRulesDataSourceSelectorExtensions
    {
        private static RulesFrameworkDbContext rulesFrameworkDbContext = null;

        public static RulesFrameworkDbContext RulesFrameworkDbContext
        {
            get
            {
                rulesFrameworkDbContext ??= new RulesFrameworkDbContext();
                return rulesFrameworkDbContext;
            }
        }

        /// <summary>
        /// Sets the rules engine data source from a Sql Server database.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="sqlServerConnection">The connection string for</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetSqlServerDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector,
            string sqlServerConnection)
        {
            if (rulesDataSourceSelector is null)
            {
                throw new ArgumentNullException(nameof(rulesDataSourceSelector));
            }

            if (sqlServerConnection is null)
            {
                throw new ArgumentNullException(nameof(sqlServerConnection));
            }

            var contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider<TContentType>();

            var ruleFactory = new RuleFactory<TContentType, TConditionType>(contentSerializationProvider);

            var sqlServerDbProviderRulesDataSource = new SqlServerProviderRulesDataSource<TContentType, TConditionType>(RulesFrameworkDbContext, ruleFactory);

            return rulesDataSourceSelector.SetDataSource(sqlServerDbProviderRulesDataSource);
        }
    }
}