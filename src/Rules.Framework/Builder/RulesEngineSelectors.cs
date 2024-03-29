namespace Rules.Framework.Builder
{
    using System;

    internal static class RulesEngineSelectors
    {
        internal sealed class ConditionTypeSelector<TContentType> : IConditionTypeSelector<TContentType>
        {
            public IRulesDataSourceSelector<TContentType, TConditionType> WithConditionType<TConditionType>()
                => new RulesDataSourceSelector<TContentType, TConditionType>();
        }

        internal sealed class ContentTypeSelector : IContentTypeSelector
        {
            public IConditionTypeSelector<TContentType> WithContentType<TContentType>() => new ConditionTypeSelector<TContentType>();
        }

        internal sealed class RulesDataSourceSelector<TContentType, TConditionType> : IRulesDataSourceSelector<TContentType, TConditionType>
        {
            public IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetDataSource(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
            {
                if (rulesDataSource == null)
                {
                    throw new ArgumentNullException(nameof(rulesDataSource));
                }

                return new ConfiguredRulesEngineBuilder<TContentType, TConditionType>(rulesDataSource);
            }
        }
    }
}