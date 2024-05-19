namespace Rules.Framework.Rql.Ast.Statements
{
    internal interface IStatementVisitor<out T>
    {
        T VisitExpressionStatement(ExpressionStatement expressionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);
    }
}