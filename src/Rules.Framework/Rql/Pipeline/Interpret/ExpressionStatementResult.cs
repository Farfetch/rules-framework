namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal class ExpressionStatementResult : IResult
    {
        public ExpressionStatementResult(string rql, object result)
        {
            this.Rql = rql;
            this.Result = result;
        }

        public bool HasOutput => true;

        public object Result { get; }

        public string Rql { get; }

        public bool Success => true;
    }
}