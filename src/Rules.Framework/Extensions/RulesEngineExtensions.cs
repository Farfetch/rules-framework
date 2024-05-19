namespace Rules.Framework.Extension
{
    using System;
    using Rules.Framework.Generics;

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
        public static IGenericRulesEngine CreateGenericEngine<TContentType, TConditionType>(this RulesEngine<TContentType, TConditionType> rulesEngine)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new NotSupportedException($"Generic rules engine is only supported for enum types of {nameof(TContentType)}.");
            }

            return new GenericRulesEngine<TContentType, TConditionType>(rulesEngine);
        }
    }
}