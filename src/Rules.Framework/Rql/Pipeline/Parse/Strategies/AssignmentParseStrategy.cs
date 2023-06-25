namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class AssignmentParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public AssignmentParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var expression = this.ParseExpressionWith<ObjectParseStrategy>(parseContext);

            if (parseContext.MoveNextIfNextToken(TokenType.ASSIGN))
            {
                var assignmentToken = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();

                var rightExpression = this.ParseExpressionWith<AssignmentParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                if (expression is VariableExpression variableExpression)
                {
                    return new AssignmentExpression(variableExpression.Token, assignmentToken, rightExpression);
                }

                if (expression is PropertyGetExpression propertyGetExpression)
                {
                    return new PropertySetExpression(propertyGetExpression.Instance, propertyGetExpression.Name, assignmentToken, rightExpression);
                }

                parseContext.EnterPanicMode("Expected assign variable or property set expression.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return expression;
        }
    }
}