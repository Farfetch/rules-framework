namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ErrorStatementResult : IResult
    {
        public ErrorStatementResult(string message, string rql, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.Message = message;
            this.Rql = rql;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public bool HasOutput => false;

        public string Message { get; }

        public string Rql { get; }

        public bool Success => false;
    }
}