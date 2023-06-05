namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitQueryStatement(QueryStatement matchStatementt);

        T VisitNoneStatement(NoneStatement noneStatement);
    }
}