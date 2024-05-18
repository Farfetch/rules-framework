namespace Rules.Framework.Rql.Ast.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitExpressionStatement(ExpressionStatement expressionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);
    }
}