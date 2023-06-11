namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitActivationStatement(ActivationStatement activationStatement);

        T VisitCreateStatement(CreateStatement createStatement);

        T VisitDeactivationStatement(DeactivationStatement deactivationStatement);

        T VisitDefinitionStatement(DefinitionStatement definitionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitQueryStatement(QueryStatement queryStatement);

        T VisitUpdateStatement(UpdateStatement updateStatement);
    }
}