namespace Rules.Framework.Providers.InMemory
{
    using System;
    using Rules.Framework.Builder;

    public static class InMemoryProviderRulesDataSourceSelectorExtensions
    {
        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetInMemoryDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector)
            => rulesDataSourceSelector.SetInMemoryDataSource(new InMemoryRulesStorage<TContentType, TConditionType>());

        public static IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetInMemoryDataSource<TContentType, TConditionType>(
            this IRulesDataSourceSelector<TContentType, TConditionType> rulesDataSourceSelector,
            IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            IInMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage = serviceProvider
                .GetService(typeof(InMemoryRulesStorage<TContentType, TConditionType>)) as InMemoryRulesStorage<TContentType, TConditionType>;

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

            IRuleFactory<TContentType, TConditionType> ruleFactory = new RuleFactory<TContentType, TConditionType>();
            InMemoryProviderRulesDataSource<TContentType, TConditionType> inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<TContentType, TConditionType>(inMemoryRulesStorage, ruleFactory);

            return rulesDataSourceSelector.SetDataSource(inMemoryProviderRulesDataSource);
        }
    }
}