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

    internal class RqlEngine<TContentType, TConditionType> : IRqlEngine
    {
        private const string ExceptionMessage = "Errors have occurred processing provided RQL source";
        private const string RqlErrorSourceUnavailable = "<unavailable>";
        private bool disposedValue;
        private IInterpreter interpreter;
        private IParser parser;
        private IScanner scanner;

        public RqlEngine(RqlEngineArgs rqlEngineArgs)
        {
            this.scanner = rqlEngineArgs.Scanner;
            this.parser = rqlEngineArgs.Parser;
            this.interpreter = rqlEngineArgs.Interpreter;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<IResult>> ExecuteAsync(string rql)
        {
            var scanResult = this.scanner.ScanTokens(rql);
            if (!scanResult.Success)
            {
                var errors = scanResult.Messages.Where(m => m.Severity == MessageSeverity.Error)
                    .Select(m => new RqlError(m.Text, RqlErrorSourceUnavailable, m.BeginPosition, m.EndPosition))
                    .ToArray();
                throw new RqlException(ExceptionMessage, errors);
            }

            var tokens = scanResult.Tokens;
            var parserResult = parser.Parse(tokens);
            if (!parserResult.Success)
            {
                var errors = parserResult.Messages.Where(m => m.Severity == MessageSeverity.Error)
                    .Select(m => new RqlError(m.Text, RqlErrorSourceUnavailable, m.BeginPosition, m.EndPosition))
                    .ToArray();
                throw new RqlException(ExceptionMessage, errors);
            }

            var statements = parserResult.Statements;
            var interpretResult = await interpreter.InterpretAsync(statements).ConfigureAwait(false);
            if (interpretResult.Success)
            {
                return interpretResult.Results.Select(s => ConvertResult(s)).ToArray();
            }

            var errorResults = interpretResult.Results.Where(s => s is ErrorStatementResult)
                .Cast<ErrorStatementResult>()
                .Select(s => new RqlError(s.Message, s.Rql, s.BeginPosition, s.EndPosition));
            throw new RqlException(ExceptionMessage, errorResults);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.interpreter = null!;
                    this.scanner = null!;
                    this.parser = null!;
                }

                disposedValue = true;
            }
        }

        private static IResult ConvertResult(Pipeline.Interpret.IResult result) => result switch
        {
            NothingStatementResult nothingStatementResult => new NothingResult(nothingStatementResult.Rql),
            ExpressionStatementResult expressionStatementResult when IsRulesSetResult(expressionStatementResult) => ConvertToRulesSetResult(expressionStatementResult),
            ExpressionStatementResult expressionStatementResult => new ValueResult(expressionStatementResult.Rql, expressionStatementResult.Result),
            _ => throw new NotSupportedException($"Result of type '{result.GetType().FullName}' is not supported."),
        };

        private static RulesSetResult<TContentType, TConditionType> ConvertToRulesSetResult(ExpressionStatementResult expressionStatementResult)
        {
            var rqlArray = (RqlArray)expressionStatementResult.Result;
            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(rqlArray.Size);
            for (int i = 0; i < rqlArray.Size; i++)
            {
                var rule = rqlArray.Value[i].Unwrap<RqlRule<TContentType, TConditionType>>();
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