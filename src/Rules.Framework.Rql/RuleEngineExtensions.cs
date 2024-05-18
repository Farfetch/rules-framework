namespace Rules.Framework.Rql
{
    using System;

    public static class RuleEngineExtensions
    {
        public static IRqlEngine GetRqlEngine<TContentType, TConditionType>(this IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            return rulesEngine.GetRqlEngine(RqlOptions.NewWithDefaults());
        }

        public static IRqlEngine GetRqlEngine<TContentType, TConditionType>(this IRulesEngine<TContentType, TConditionType> rulesEngine, RqlOptions rqlOptions)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new NotSupportedException($"Rule Query Language is not supported for non-enum types of {nameof(TContentType)}.");
            }

            if (!typeof(TConditionType).IsEnum)
            {
                throw new NotSupportedException($"Rule Query Language is not supported for non-enum types of {nameof(TConditionType)}.");
            }

            return RqlEngineBuilder<TContentType, TConditionType>.CreateRqlEngine(rulesEngine)
                .WithOptions(rqlOptions)
                .Build();
        }
    }
}