namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.SqlServer.Serialization;
    using Rules.Framework.Serialization;

    /// <summary>
    /// Rules data source selector extensions from SQL Server database provider.
    /// </summary>
    public static class SqlServerRulesDataSourceSelectorExtensions
    {
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
            //TODO: check if we use Guard
            if (rulesDataSourceSelector is null)
            {
                throw new ArgumentNullException(nameof(rulesDataSourceSelector));
            }

            if (sqlServerConnection is null)
            {
                throw new ArgumentNullException(nameof(sqlServerConnection));
            }

            IContentSerializationProvider<TContentType> contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider<TContentType>();
            IRuleFactory<TContentType, TConditionType> ruleFactory = new RuleFactory<TContentType, TConditionType>(contentSerializationProvider);
            SqlServerProviderRulesDataSource<TContentType, TConditionType> mongoDbProviderRulesDataSource
                = new SqlServerProviderRulesDataSource<TContentType, TConditionType>(
                    sqlServerConnection,
                    ruleFactory);

            return rulesDataSourceSelector.SetDataSource(mongoDbProviderRulesDataSource);
        }
    }
}