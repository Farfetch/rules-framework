namespace Rules.Framework
{
    using System;
    using Rules.Framework.Rql;

    public static class RulesEngineExtensions
    {
        public static IRqlEngine GetRqlEngine(this IRulesEngine rulesEngine)
        {
            return rulesEngine.GetRqlEngine(RqlOptions.NewWithDefaults());
        }

        public static IRqlEngine GetRqlEngine(this IRulesEngine rulesEngine, RqlOptions rqlOptions)
        {
            return RqlEngineBuilder.CreateRqlEngine(rulesEngine)
                .WithOptions(rqlOptions)
                .Build();
        }

        private static bool IsSupportedType(Type type)
        {
            return type.IsEnum || type == typeof(string);
        }
    }
}