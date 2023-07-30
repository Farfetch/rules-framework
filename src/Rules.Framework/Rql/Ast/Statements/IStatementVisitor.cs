namespace Rules.Framework.Rql.Ast.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitBlockStatement(BlockStatement block);

        T VisitExpressionStatement(ExpressionStatement expressionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement);
    }
}