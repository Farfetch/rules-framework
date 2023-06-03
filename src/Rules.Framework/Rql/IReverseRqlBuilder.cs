namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;

    internal interface IReverseRqlBuilder
    {
        string BuildRql(Expression expression);

        string BuildRql(Statement statement);
    }
}