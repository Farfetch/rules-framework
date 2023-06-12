namespace Rules.Framework.Rql
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Rules.Framework.Source;

    internal class RqlClient<TContentType, TConditionType> : IRqlClient<TContentType, TConditionType>
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;

        public RqlClient(
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IRulesSource<TContentType, TConditionType> rulesSource)
        {
            this.rulesEngine = rulesEngine;
            this.rulesSource = rulesSource;
        }

        public async Task<IEnumerable<ResultSet<TContentType, TConditionType>>> ExecuteQueryAsync(string rql)
        {
            var scanner = new Scanner();
            var scanResult = scanner.ScanTokens(rql);

            if (!scanResult.Success)
            {
                var errors = scanResult.Messages.Where(m => m.Severity == MessageSeverity.Error)
                    .Select(m => new RqlError(m.Text, "<unavailable>", m.BeginPosition, m.EndPosition))
                    .ToArray();
                throw new RqlException("Errors have occurred processing provided RQL source.", errors);
            }

            var tokens = scanResult.Tokens;
            var parseStrategyProvider = new ParseStrategyPool();
            var parser = new Parser(parseStrategyProvider);
            var parserResult = parser.Parse(tokens);

            if (!parserResult.Success)
            {
                var errors = parserResult.Messages.Where(m => m.Severity == MessageSeverity.Error)
                    .Select(m => new RqlError(m.Text, "<unavailable>", m.BeginPosition, m.EndPosition))
                    .ToArray();
                throw new RqlException("Errors have occurred processing provided RQL source.", errors);
            }

            var statements = parserResult.Statements;
            using var runtimeEnvironment = new RuntimeEnvironment().Initialize();
            var reverseRqlBuilder = new ReverseRqlBuilder();
            var interpreter = new Interpreter<TContentType, TConditionType>(this.rulesEngine, this.rulesSource, runtimeEnvironment, reverseRqlBuilder);
            var interpretResult = (InterpretResult)await interpreter.InterpretAsync(statements).ConfigureAwait(false);

            if (interpretResult.Success)
            {
                return interpretResult.Results
                    .Cast<ResultSetStatementResult<TContentType, TConditionType>>()
                    .Select(s => s.ResultSet);
            }

            var errorResults = interpretResult.Results.Where(s => s is ErrorResult)
                .Cast<ErrorResult>()
                .Select(s => new RqlError(s.Message, s.Rql, s.BeginPosition, s.EndPosition));
            throw new RqlException("Errors have occurred processing provided RQL source.", errorResults);
        }
    }
}