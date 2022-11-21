namespace Rules.Framework.Extensions
{
    using System;
    using System.Linq;
    using Rules.Framework.Generic;

    internal static class GenericSearchArgsExtensions
    {
        public static SearchArgs<TContentType, TConditionType> ToSearchArgs<TContentType, TConditionType>(
            this SearchArgs<GenericContentType, GenericConditionType> genericSearchArgs)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new ArgumentException("Only TContentType of type enum are currently supported.");
            }

            var contentType = (TContentType)Enum.Parse(typeof(TContentType), genericSearchArgs.ContentType.Name);

            var searchArgs = new SearchArgs<TContentType, TConditionType>(contentType, genericSearchArgs.DateBegin, genericSearchArgs.DateEnd)
            {
                Conditions = genericSearchArgs.Conditions.Select(condition => new Condition<TConditionType>
                {
                    Value = condition.Value,
                    Type = (TConditionType)Enum.Parse(typeof(TConditionType), condition.Type.Name)
                }),
                ExcludeRulesWithoutSearchConditions = genericSearchArgs.ExcludeRulesWithoutSearchConditions
            };

            return searchArgs;
        }
    }
}