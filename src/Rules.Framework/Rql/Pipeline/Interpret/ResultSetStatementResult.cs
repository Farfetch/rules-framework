namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal class ResultSetStatementResult<TContentType, TConditionType> : IResult
    {
        public ResultSetStatementResult(ResultSet<TContentType, TConditionType> resultSet)
        {
            this.ResultSet = resultSet;
        }

        public bool HasOutput => true;

        public ResultSet<TContentType, TConditionType> ResultSet { get; }

        public bool Success => true;
    }
}