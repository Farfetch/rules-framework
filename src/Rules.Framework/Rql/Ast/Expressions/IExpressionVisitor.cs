namespace Rules.Framework.Rql.Ast.Expressions
{
    internal interface IExpressionVisitor<T>
    {
        T VisitActivationExpression(ActivationExpression activationExpression);

        T VisitAssignmentExpression(AssignmentExpression expression);

        T VisitBinaryExpression(BinaryExpression binaryExpression);

        T VisitCallExpression(CallExpression callExpression);

        T VisitCreateExpression(CreateExpression createExpression);

        T VisitDeactivationExpression(DeactivationExpression deactivationExpression);

        T VisitIdentifierExpression(IdentifierExpression identifierExpression);

        T VisitIndexerGetExpression(IndexerGetExpression indexerGetExpression);

        T VisitIndexerSetExpression(IndexerSetExpression indexerSetExpression);

        T VisitKeywordExpression(KeywordExpression keywordExpression);

        T VisitLiteralExpression(LiteralExpression literalExpression);

        T VisitMatchExpression(MatchExpression matchExpression);

        T VisitNewArrayExpression(NewArrayExpression newArrayExpression);

        T VisitNewObjectExpression(NewObjectExpression newObjectExpression);

        T VisitNoneExpression(NoneExpression noneExpression);

        T VisitPlaceholderExpression(PlaceholderExpression placeholderExpression);

        T VisitPropertyGetExpression(PropertyGetExpression propertyGetExpression);

        T VisitPropertySetExpression(PropertySetExpression propertySetExpression);

        T VisitSearchExpression(SearchExpression searchExpression);

        T VisitUnaryExpression(UnaryExpression expression);

        T VisitUpdateExpression(UpdateExpression updateExpression);

        T VisitVariableDeclarationExpression(VariableDeclarationExpression variableDeclarationExpression);

        T VisitVariableExpression(VariableExpression variableExpression);
    }
}