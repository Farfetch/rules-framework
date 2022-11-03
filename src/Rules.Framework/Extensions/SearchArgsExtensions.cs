namespace Rules.Framework.Extensions
{
    using System;
    using System.Linq;

    internal static class SearchArgsExtensions
    {
        public static SearchArgs<TContentType, TConditionType> ToSearchArgs<TContentType, TConditionType>(
            this SearchArgs<ContentType, ConditionType> searchArgs)
        {
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), searchArgs.ContentType.Name);

            var genericSearchArgs = new SearchArgs<TContentType, TConditionType>(
                contentType,
                searchArgs.DateBegin,
                searchArgs.DateEnd);

            genericSearchArgs.Conditions = searchArgs.Conditions.Select(condition => new Condition<TConditionType>
            {
                Value = condition.Value,
                Type = (TConditionType)Enum.Parse(typeof(TConditionType), condition.Type.Code)
            });

            return genericSearchArgs;
        }
    }
}