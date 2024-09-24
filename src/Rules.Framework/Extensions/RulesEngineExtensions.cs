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
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <returns>A new instance of generic engine</returns>
        public static IRulesEngine<TContentType, TConditionType> MakeGeneric<TContentType, TConditionType>(this IRulesEngine rulesEngine)
        {
            return new RulesEngine<TContentType, TConditionType>(rulesEngine);
        }
    }
}