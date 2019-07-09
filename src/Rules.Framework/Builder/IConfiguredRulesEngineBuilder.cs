namespace Rules.Framework.Builder
{
    using System;

    /// <summary>
    /// Exposes the interface contract for a configured rules engine builder. Allows to perform additional optional configurations and finish rules engine build.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    public interface IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        /// <summary>
        /// Builds a rules engine instance using all supplied configuration options.
        /// </summary>
        /// <returns>the rules engine instance.</returns>
        RulesEngine<TContentType, TConditionType> Build();

        /// <summary>
        /// Allows configuration of rules engine options.
        /// </summary>
        /// <param name="configurationAction">the action with configuration logic for rules engine options.</param>
        /// <returns>the configured rules engine builder.</returns>
        IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction);
    }
}