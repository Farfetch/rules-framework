namespace Rules.Framework
{
    using Rules.Framework.Rql;

    /// <summary>
    /// Extensions for rules engine
    /// </summary>
    public static class RulesEngineExtensions
    {
        /// <summary>
        /// Gets the Rule Query Language engine, using the rules engine.
        /// </summary>
        /// <param name="rulesEngine">The rules engine to be used by the Rule Query Language engine.</param>
        /// <returns>a new Rule Query Language engine.</returns>
        public static IRqlEngine GetRqlEngine(this IRulesEngine rulesEngine)
        {
            return rulesEngine.GetRqlEngine(RqlOptions.NewWithDefaults());
        }

        /// <summary>
        /// Gets the Rule Query Language engine, using the rules engine.
        /// </summary>
        /// <param name="rulesEngine">The rules engine to be used by the Rule Query Language engine.</param>
        /// <param name="rqlOptions">The Rule Query Language engine options.</param>
        /// <returns>a new Rule Query Language engine.</returns>
        public static IRqlEngine GetRqlEngine(this IRulesEngine rulesEngine, RqlOptions rqlOptions)
        {
            return RqlEngineBuilder.CreateRqlEngine(rulesEngine)
                .WithOptions(rqlOptions)
                .Build();
        }
    }
}