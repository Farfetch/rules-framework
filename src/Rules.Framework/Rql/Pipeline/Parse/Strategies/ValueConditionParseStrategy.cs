namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ValueConditionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ValueConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.PLACEHOLDER))
            {
                parseContext.EnterPanicMode("Expect name for condition", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var leftToken = parseContext.GetCurrentToken();
            var leftExpression = new PlaceholderExpression(leftToken);

            var operatorExpression = this.ParseExpressionWith<OperatorParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected value for condition.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var rightExpression = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new ValueConditionExpression(leftExpression, operatorExpression, rightExpression);
        }
    }
}