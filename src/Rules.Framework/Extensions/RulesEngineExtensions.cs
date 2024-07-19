namespace Rules.Framework
{
    using Rules.Framework.Generic;

    /// <summary>
    /// Extensions for rules engine
    /// </summary>
    public static class RulesEngineExtensions
    {
        /// <summary>
        /// Creates a generic rules engine.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <returns>A new instance of generic engine</returns>
        public static IRulesEngine<TRuleset, TCondition> MakeGeneric<TRuleset, TCondition>(this IRulesEngine rulesEngine)
        {
            return new RulesEngine<TRuleset, TCondition>(rulesEngine);
        }
    }
}