namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitCreateStatement(CreateStatement createStatement);

        T VisitDefinitionStatement(DefinitionStatement definitionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitQueryStatement(QueryStatement queryStatement);

        T VisitUpdateStatement(UpdateStatement updateStatement);
    }
}