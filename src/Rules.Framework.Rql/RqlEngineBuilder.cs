namespace Rules.Framework.Rql
{
    using System;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Rules.Framework.Rql.Runtime;

    internal sealed class RqlEngineBuilder
    {
        private readonly IRulesEngine rulesEngine;
        private RqlOptions options;

        private RqlEngineBuilder(IRulesEngine rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        public static RqlEngineBuilder CreateRqlEngine(IRulesEngine rulesEngine)
        {
            if (rulesEngine is null)
            {
                throw new ArgumentNullException(nameof(rulesEngine));
            }

            return new RqlEngineBuilder(rulesEngine);
        }

        public IRqlEngine Build()
        {
            var runtime = RqlRuntime.Create(this.rulesEngine);
            var tokenScanner = new TokenScanner();
            var parseStrategyProvider = new ParseStrategyPool();
            var parser = new Parser(parseStrategyProvider);
            var reverseRqlBuilder = new ReverseRqlBuilder();
            var interpreter = new Interpreter(runtime, reverseRqlBuilder);
            var args = new RqlEngineArgs
            {
                Interpreter = interpreter,
                Options = this.options,
                Parser = parser,
                TokenScanner = tokenScanner,
            };

            return new RqlEngine(args);
        }

        public RqlEngineBuilder WithOptions(RqlOptions options)
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