namespace Rules.Framework.Extensions
{
    using Rules.Framework.Core;
    using Rules.Framework.Generic;

    internal static class GenericRuleExtensions
    {
        public static GenericRule ToGenericRule<TContentType, TConditionType>(this Rule<TContentType, TConditionType> rule)
        {
            return new GenericRule
            {
                //RootCondition = new ConditionNode<ConditionType>(),
                //ContentContainer = new ContentContainer<ContentType>(ContentType, rule.ContentContainer.GetContentAs<ContentType>()),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority
            };
        }
    }
}