namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;

    internal class RulesSetStatementResult<TContentType, TConditionType> : IResult
    {
        public RulesSetStatementResult(RulesSetResult<TContentType, TConditionType> resultSet)
        {
            this.ResultSet = resultSet ?? throw new ArgumentNullException(nameof(resultSet));
        }

        public bool HasOutput => true;

        public RulesSetResult<TContentType, TConditionType> ResultSet { get; }

        public string Rql => this.ResultSet.Rql;

        public bool Success => true;
    }
}