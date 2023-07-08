namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Statements;

    internal interface IReverseRqlBuilder
    {
        string BuildRql(Expression expression);

        string BuildRql(Statement statement);
    }
}