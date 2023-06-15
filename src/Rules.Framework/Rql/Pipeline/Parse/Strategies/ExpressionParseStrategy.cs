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
            if (parseContext.IsMatchCurrentToken(TokenType.IDENTIFIER))
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

                return new VariableExpression(parseContext.GetCurrentToken());
            }

            if (parseContext.IsMatchCurrentToken(TokenType.STRING, TokenType.INT, TokenType.BOOL, TokenType.DECIMAL, TokenType.NOTHING))
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