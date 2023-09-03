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
        private IRulesSource<TContentType, TConditionType> rulesSource;

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
            var callableTable = new RqlCallableTable().Initialize(this.options);
            var runtimeEnvironment = new RqlEnvironment();
            var runtime = RqlRuntime<TContentType, TConditionType>.Create(callableTable, runtimeEnvironment, this.rulesEngine, this.rulesSource);
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

        public RqlEngineBuilder<TContentType, TConditionType> WithRulesSource(IRulesSource<TContentType, TConditionType> rulesSource)
        {
            if (rulesSource is null)
            {
                throw new ArgumentNullException(nameof(rulesSource));
            }

            this.rulesSource = rulesSource;
            return this;
        }
    }
}