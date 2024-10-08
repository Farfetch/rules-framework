namespace Rules.Framework.Builder
{
    using System;

    /// <summary>
    /// Exposes the interface contract for a configured rules engine builder. Allows to perform
    /// additional optional configurations and finish rules engine build.
    /// </summary>
    public interface IConfiguredRulesEngineBuilder
    {
        /// <summary>
        /// Builds a rules engine instance using all supplied configuration options.
        /// </summary>
        /// <returns>the rules engine instance.</returns>
        IRulesEngine Build();

        /// <summary>
        /// Allows configuration of rules engine options.
        /// </summary>
        /// <param name="configurationAction">
        /// the action with configuration logic for rules engine options.
        /// </param>
        /// <returns>the configured rules engine builder.</returns>
        IConfiguredRulesEngineBuilder Configure(Action<RulesEngineOptions> configurationAction);
    }
}