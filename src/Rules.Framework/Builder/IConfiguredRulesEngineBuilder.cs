namespace Rules.Framework.Builder
{
    using System;

    public interface IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        RulesEngine<TContentType, TConditionType> Build();

        IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction);
    }
}