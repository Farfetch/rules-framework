namespace Rules.Framework.Providers.InMemory
{
    using System;
    using Rules.Framework.Builder;

    /// <summary>
    /// Rules data source selector extensions from in-memory provider.
    /// </summary>
    public static class InMemoryProviderRulesDataSourceSelectorExtensions
    {
        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector</exception>
        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetInMemoryDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector)
            => rulesDataSourceSelector.SetInMemoryDataSource(new InMemoryRulesStorage<TContentType, TConditionType>());

        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector or serviceProvider</exception>
        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetInMemoryDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector,
            IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var inMemoryRulesStorage = (IInMemoryRulesStorage<TContentType, TConditionType>)serviceProvider
                .GetService(typeof(IInMemoryRulesStorage<TContentType, TConditionType>));

            return rulesDataSourceSelector.SetInMemoryDataSource(inMemoryRulesStorage);
        }

        private static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetInMemoryDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector,
            IInMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage)
        {
            if (rulesDataSourceSelector is null)
            {
                throw new ArgumentNullException(nameof(rulesDataSourceSelector));
            }

            var ruleFactory = new RuleFactory<TContentType, TConditionType>();
            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<TContentType, TConditionType>(inMemoryRulesStorage, ruleFactory);

            return rulesDataSourceSelector.SetDataSource(inMemoryProviderRulesDataSource);
        }
    }
}