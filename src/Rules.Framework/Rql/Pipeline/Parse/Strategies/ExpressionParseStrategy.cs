namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ExpressionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ExpressionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
            {
                if (parseContext.IsMatchNextToken(TokenType.PARENTHESIS_LEFT))
                {
                    var call = this.ParseExpressionWith<CallParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return call;
                }

                // TODO: add future support.
            }

            if (parseContext.MoveNextIfNextToken(TokenType.STRING, TokenType.INT, TokenType.BOOL, TokenType.DECIMAL))
            {
                var literal = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return literal;
            }

            parseContext.EnterPanicMode("Expected expression.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}