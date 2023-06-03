namespace Rules.Framework.Rql.Expressions
{
    internal interface IExpressionVisitor<T>
    {
        T VisitCardinalityExpression(CardinalityExpression expression);

        T VisitInputConditionExpression(InputConditionExpression inputConditionExpression);

        T VisitKeywordExpression(KeywordExpression keywordExpression);

        T VisitLiteralExpression(LiteralExpression literalExpression);

        T VisitMatchExpression(MatchExpression matchExpression);

        T VisitNoneExpression(NoneExpression noneExpression);
    }
}