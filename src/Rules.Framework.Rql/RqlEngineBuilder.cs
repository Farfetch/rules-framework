namespace Rules.Framework.Rql
{
    using System;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Source;

    internal class RqlEngineBuilder<TContentType, TConditionType>
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;
        private RqlOptions options;

        private RqlEngineBuilder(IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        public static RqlEngineBuilder<TContentType, TConditionType> CreateRqlEngine(IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            if (rulesEngine is null)
            {
                throw new ArgumentNullException(nameof(rulesEngine));
            }

            return new RqlEngineBuilder<TContentType, TConditionType>(rulesEngine);
        }

        public IRqlEngine Build()
        {
            var runtime = RqlRuntime<TContentType, TConditionType>.Create(this.rulesEngine);
            var scanner = new Scanner();
            var parseStrategyProvider = new ParseStrategyPool();
            var parser = new Parser(parseStrategyProvider);
            var reverseRqlBuilder = new ReverseRqlBuilder();
            var interpreter = new Interpreter<TContentType, TConditionType>(runtime, reverseRqlBuilder);
            var args = new RqlEngineArgs
            {
                Interpreter = interpreter,
                Options = this.options,
                Parser = parser,
                Scanner = scanner,
            };

            return new RqlEngine<TContentType, TConditionType>(args);
        }

        public RqlEngineBuilder<TContentType, TConditionType> WithOptions(RqlOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options;
            return this;
        }
    }
}