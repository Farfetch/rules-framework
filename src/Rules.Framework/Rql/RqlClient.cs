namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Rules.Framework.Rql.Runtime.Types;

    internal class RqlClient<TContentType, TConditionType> : IRqlClient<TContentType, TConditionType>
    {
        private readonly IInterpreter interpreter;
        private bool disposedValue;

        public RqlClient(IInterpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<IResult>> ExecuteAsync(string rql)
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
            var interpretResult = (InterpretResult)await this.interpreter.InterpretAsync(statements).ConfigureAwait(false);

            if (interpretResult.Success)
            {
                return interpretResult.Results.Select(s => ConvertResult(s));
            }

            var errorResults = interpretResult.Results.Where(s => s is ErrorResult)
                .Cast<ErrorResult>()
                .Select(s => new RqlError(s.Message, s.Rql, s.BeginPosition, s.EndPosition));
            throw new RqlException("Errors have occurred processing provided RQL source.", errorResults);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.interpreter.Dispose();
                }

                disposedValue = true;
            }
        }

        private static IResult ConvertResult(Pipeline.Interpret.IResult result) => result switch
        {
            RulesSetStatementResult<TContentType, TConditionType> rulesSetStatementResult => rulesSetStatementResult.ResultSet,
            NothingStatementResult nothingStatementResult => new NothingResult(nothingStatementResult.Rql),
            ExpressionResult expressionResult when expressionResult.Result is RqlNothing => new NothingResult(expressionResult.Rql),
            ExpressionResult expressionResult => new ValueResult(expressionResult.Rql, expressionResult.Result),
            _ => throw new NotSupportedException($"Result of type '{result.GetType().FullName}' is not supported."),
        };
    }
}