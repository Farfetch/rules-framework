namespace Rules.Framework.Extensions
{
    using System;
    using System.Linq;
    using Rules.Framework.Generic;

    internal static class GenericSearchArgsExtensions
    {
        public static SearchArgs<TContentType, TConditionType> ToSearchArgs<TContentType, TConditionType>(
            this SearchArgs<GenericContentType, GenericConditionType> searchArgs)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new ArgumentException("Extensions only works if TContentType is a enum");
            }

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