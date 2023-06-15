namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal class NothingStatementResult : IResult
    {
        public NothingStatementResult(string rql)
        {
            if (string.IsNullOrWhiteSpace(rql))
            {
                throw new System.ArgumentException($"'{nameof(rql)}' cannot be null or whitespace.", nameof(rql));
            }

            this.Rql = rql;
        }

        public bool HasOutput => false;

        public string Rql { get; }

        public bool Success => true;
    }
}