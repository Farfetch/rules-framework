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

            var errorResults = interpretResult.Results.Where(s => s is ErrorStatementResult)
                .Cast<ErrorStatementResult>()
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
            NothingStatementResult nothingStatementResult => new NothingResult(nothingStatementResult.Rql),
            ExpressionStatementResult expressionStatementResult when IsRulesSetResult(expressionStatementResult) => ConvertToRulesSetResult(expressionStatementResult),
            ExpressionStatementResult expressionStatementResult when expressionStatementResult.Result is RqlNothing => new NothingResult(expressionStatementResult.Rql),
            ExpressionStatementResult expressionStatementResult => new ValueResult(expressionStatementResult.Rql, expressionStatementResult.Result),
            _ => throw new NotSupportedException($"Result of type '{result.GetType().FullName}' is not supported."),
        };

        private static RulesSetResult<TContentType, TConditionType> ConvertToRulesSetResult(ExpressionStatementResult expressionStatementResult)
        {
            var rqlArray = (RqlArray)expressionStatementResult.Result;
            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(rqlArray.Size);
            for (int i = 0; i < rqlArray.Size; i++)
            {
                var rule = rqlArray.Value[i].Unwrap<RqlRule<TContentType, TConditionType>>().Value;
                var rulesSetResultLine = new RulesSetResultLine<TContentType, TConditionType>(i + 1, rule);
                lines.Add(rulesSetResultLine);
            }

            return new RulesSetResult<TContentType, TConditionType>(expressionStatementResult.Rql, rqlArray.Size, lines);
        }

        private static bool IsRulesSetResult(ExpressionStatementResult expressionStatementResult)
        {
            if (expressionStatementResult.Result is RqlArray rqlArray)
            {
                if (rqlArray.Size <= 0)
                {
                    return false;
                }

                for (int i = 0; i < rqlArray.Size; i++)
                {
                    if (rqlArray.Value[i].UnderlyingType != RqlTypes.Rule)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}