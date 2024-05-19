namespace Rules.Framework
{
    using System;
    using Rules.Framework.Rql;

    public static class RulesEngineExtensions
    {
        public static IRqlEngine GetRqlEngine<TContentType, TConditionType>(this IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            return rulesEngine.GetRqlEngine(RqlOptions.NewWithDefaults());
        }

        public static IRqlEngine GetRqlEngine<TContentType, TConditionType>(this IRulesEngine<TContentType, TConditionType> rulesEngine, RqlOptions rqlOptions)
        {
            if (!IsSupportedType(typeof(TContentType)))
            {
                throw new NotSupportedException($"Rule Query Language is only supported for enum types or strings on {nameof(TContentType)}.");
            }

            if (!IsSupportedType(typeof(TConditionType)))
            {
                throw new NotSupportedException($"Rule Query Language is only supported for enum types or strings on {nameof(TConditionType)}.");
            }

            return RqlEngineBuilder<TContentType, TConditionType>.CreateRqlEngine(rulesEngine)
                .WithOptions(rqlOptions)
                .Build();
        }

        private static bool IsSupportedType(Type type)
        {
            return type.IsEnum || type == typeof(string);
        }
    }
}