namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal class ExpressionStatementResult : IResult
    {
        public ExpressionStatementResult(string rql, object result)
        {
            if (string.IsNullOrWhiteSpace(rql))
            {
                throw new System.ArgumentException($"'{nameof(rql)}' cannot be null or whitespace.", nameof(rql));
            }

            this.Rql = rql;
            this.Result = result;
        }

        public bool HasOutput => true;

        public object Result { get; }

        public string Rql { get; }

        public bool Success => true;
    }
}