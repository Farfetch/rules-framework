namespace Rules.Framework.Extensions
{
    using Rules.Framework.Generic;

    public static class RulesEngineExtensions
    {
        public static IGenericRulesEngine CreateGenericRulesEngine<TContentType, TConditionType>(this RulesEngine<TContentType, TConditionType> rulesEngine)
        {
            return new GenericRulesEngine<TContentType, TConditionType>(rulesEngine);
        }
    }
}