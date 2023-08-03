namespace Rules.Framework.Rql.Ast.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitBlockStatement(BlockStatement block);

        T VisitExpressionStatement(ExpressionStatement expressionStatement);

        T VisitForEachStatement(ForEachStatement forEachStatement);

        T VisitIfStatement(IfStatement ifStatement);

        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitVariableBootstrapStatement(VariableBootstrapStatement variableBootstrapStatement);
    }
}