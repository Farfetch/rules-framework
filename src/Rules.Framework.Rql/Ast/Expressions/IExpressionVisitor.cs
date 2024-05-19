namespace Rules.Framework.Rql.Ast.Expressions
{
    internal interface IExpressionVisitor<out T>
    {
        T VisitAssignmentExpression(AssignmentExpression assignmentExpression);

        T VisitBinaryExpression(BinaryExpression binaryExpression);

        T VisitIdentifierExpression(IdentifierExpression identifierExpression);

        T VisitKeywordExpression(KeywordExpression keywordExpression);

        T VisitLiteralExpression(LiteralExpression literalExpression);

        T VisitMatchExpression(MatchExpression matchExpression);

        T VisitNewArrayExpression(NewArrayExpression newArrayExpression);

        T VisitNewObjectExpression(NewObjectExpression newObjectExpression);

        T VisitNoneExpression(NoneExpression noneExpression);

        T VisitPlaceholderExpression(PlaceholderExpression placeholderExpression);

        T VisitSearchExpression(SearchExpression searchExpression);

        T VisitUnaryExpression(UnaryExpression expression);
    }
}