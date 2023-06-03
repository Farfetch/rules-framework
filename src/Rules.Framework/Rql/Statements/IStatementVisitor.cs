namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitMatchStatement(MatchStatement matchStatementt);

        T VisitNoneStatement(NoneStatement noneStatement);
    }
}