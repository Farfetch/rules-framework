using System;

namespace Rules.Framework.Builder
{
    public interface IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        RulesEngine<TContentType, TConditionType> Build();

        IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction);
    }
}