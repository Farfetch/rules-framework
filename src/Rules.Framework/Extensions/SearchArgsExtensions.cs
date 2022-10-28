namespace Rules.Framework.Extensions
{
    using System;
    using System.Linq;

    internal static class SearchArgsExtensions
    {
        public static SearchArgs<TContentType, TConditionType> ToSearchArgs<TContentType, TConditionType>(
            this SearchArgs<ContentType, ConditionType> genericSearchArgs)
        {
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), genericSearchArgs.ContentType.Name);

            var newSearchArgs = new SearchArgs<TContentType, TConditionType>(
                contentType,
                genericSearchArgs.DateBegin,
                genericSearchArgs.DateEnd);

            newSearchArgs.Conditions = genericSearchArgs.Conditions.Select(condition => new Condition<TConditionType>
            {
                Value = condition.Value,
                Type = (TConditionType)Enum.Parse(typeof(TConditionType), condition.Type.Code)
            });

            return newSearchArgs;
        }
    }
}