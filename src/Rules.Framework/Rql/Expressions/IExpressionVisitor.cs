namespace Rules.Framework.Rql.Expressions
{
    internal interface IExpressionVisitor<T>
    {
        T VisitAssignExpression(AssignmentExpression expression);

        T VisitCallExpression(CallExpression callExpression);

        T VisitCardinalityExpression(CardinalityExpression expression);

        T VisitComposedConditionExpression(ComposedConditionExpression expression);

        T VisitConditionGroupingExpression(ConditionGroupingExpression expression);

        T VisitInputConditionExpression(InputConditionExpression inputConditionExpression);

        T VisitInputConditionsExpression(InputConditionsExpression inputConditionsExpression);

        T VisitKeywordExpression(KeywordExpression keywordExpression);

        T VisitLiteralExpression(LiteralExpression literalExpression);

        T VisitMatchExpression(MatchExpression matchExpression);

        T VisitNewObjectExpression(NewObjectExpression newObjectExpression);

        T VisitNoneExpression(NoneExpression noneExpression);

        T VisitOperatorExpression(OperatorExpression operatorExpression);

        T VisitPlaceholderExpression(PlaceholderExpression placeholderExpression);

        T VisitPriorityOptionExpression(PriorityOptionExpression priorityOptionExpression);

        T VisitPropertyGetExpression(PropertyGetExpression propertyGetExpression);

        T VisitSearchExpression(SearchExpression searchExpression);

        T VisitUnaryExpression(UnaryExpression expression);

        T VisitUpdatableAttributeExpression(UpdatableAttributeExpression updatableExpression);

        T VisitValueConditionExpression(ValueConditionExpression valueConditionExpression);

        T VisitVariableExpression(VariableExpression variableExpression);
    }
}