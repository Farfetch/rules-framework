namespace Rules.Framework.Extensions
{
    using System;
    using System.Linq;
    using Rules.Framework.Generics;

    internal static class SearchArgsExtensions
    {
        public static SearchArgs<TContentType, TConditionType> ToGenericSearchArgs<TContentType, TConditionType>(
            this SearchArgs<string, string> genericSearchArgs)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new ArgumentException("Only TContentType of type enum are currently supported.");
            }

            var contentType = (TContentType)Enum.Parse(typeof(TContentType), genericSearchArgs.ContentType);

            if (genericSearchArgs.Active.HasValue)
            {
                return new SearchArgs<TContentType, TConditionType>(contentType, genericSearchArgs.DateBegin, genericSearchArgs.DateEnd, genericSearchArgs.Active.Value)
                {
                    Conditions = genericSearchArgs.Conditions.Select(condition => new Condition<TConditionType>
                    (
                        (TConditionType)Enum.Parse(typeof(TConditionType), condition.Type),
                        condition.Value
                    )).ToList(),
                    ExcludeRulesWithoutSearchConditions = genericSearchArgs.ExcludeRulesWithoutSearchConditions
                };
            }

            var searchArgs = new SearchArgs<TContentType, TConditionType>(contentType, genericSearchArgs.DateBegin, genericSearchArgs.DateEnd)
            {
                Conditions = genericSearchArgs.Conditions.Select(condition => new Condition<TConditionType>
                    (
                        (TConditionType)Enum.Parse(typeof(TConditionType), condition.Type),
                        condition.Value
                    )).ToList(),
                ExcludeRulesWithoutSearchConditions = genericSearchArgs.ExcludeRulesWithoutSearchConditions
            };

            return searchArgs;
        }
    }
}