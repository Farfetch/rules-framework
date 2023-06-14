namespace Rules.Framework.Rql.Statements
{
    internal interface IStatementVisitor<T>
    {
        T VisitActivationStatement(ActivationStatement activationStatement);

        T VisitCreateStatement(CreateStatement createStatement);

        T VisitDeactivationStatement(DeactivationStatement deactivationStatement);

        T VisitDefinitionStatement(RuleDefinitionStatement definitionStatement);

        T VisitNoneStatement(NoneStatement noneStatement);

        T VisitProgrammableSubLanguageStatement(ProgrammableSubLanguageStatement programmableStatement);

        T VisitQueryStatement(RuleQueryStatement queryStatement);

        T VisitUpdateStatement(UpdateStatement updateStatement);

        T VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement);
    }
}