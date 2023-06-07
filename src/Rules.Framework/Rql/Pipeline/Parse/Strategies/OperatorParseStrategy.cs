namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class OperatorParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public OperatorParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected binary operator.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var currentToken = parseContext.GetCurrentToken();
            switch (currentToken.Type)
            {
                case TokenType.EQUAL:
                case TokenType.GREATER_THAN:
                case TokenType.GREATER_THAN_OR_EQUAL:
                case TokenType.LESS_THAN:
                case TokenType.LESS_THAN_OR_EQUAL:
                case TokenType.NOT_EQUAL:
                case TokenType.IN:
                case TokenType.NOT_IN:
                    return new OperatorExpression(currentToken);

                default:
                    parseContext.EnterPanicMode("Expected binary operator.", parseContext.GetCurrentToken());
                    return Expression.None;
            }
        }
    }
}