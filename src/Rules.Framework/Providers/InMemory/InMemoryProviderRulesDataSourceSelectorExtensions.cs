namespace Rules.Framework
{
    using System;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.InMemory;

    /// <summary>
    /// Rules data source selector extensions from in-memory provider.
    /// </summary>
    public static class InMemoryProviderRulesDataSourceSelectorExtensions
    {
        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector</exception>
        public static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector)
            => rulesDataSourceSelector.SetInMemoryDataSource(new InMemoryRulesStorage());

        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector or serviceProvider</exception>
        public static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector,
            IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var inMemoryRulesStorage = (IInMemoryRulesStorage)serviceProvider
                .GetService(typeof(IInMemoryRulesStorage));

            return rulesDataSourceSelector.SetInMemoryDataSource(inMemoryRulesStorage);
        }

        private static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector,
            IInMemoryRulesStorage inMemoryRulesStorage)
        {
            if (rulesDataSourceSelector is null)
            {
                throw new ArgumentNullException(nameof(rulesDataSourceSelector));
            }

            var ruleFactory = new RuleFactory();
            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            return rulesDataSourceSelector.SetDataSource(inMemoryProviderRulesDataSource);
        }
    }
}