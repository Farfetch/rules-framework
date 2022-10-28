namespace Rules.Framework.Extensions
{
    using Rules.Framework.Core;
    using Rules.Framework.Generic;

    internal static class GenericRuleExtensions
    {
        public static GenericRule ToGenericRule<TContentType, TConditionType>(this Rule<TContentType, TConditionType> rule)
        {
            return new GenericRule();
        }
    }
}