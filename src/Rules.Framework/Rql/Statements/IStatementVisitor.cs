namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitExpressionStatement(ExpressionStatement programmableStatement);

        T VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement);
    }
}