namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class FactorParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public FactorParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var unaryExpression = this.ParseExpressionWith<UnaryParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.DIVIDE, TokenType.MULTIPLY))
            {
                var operatorToken = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();
                var rightExpression = this.ParseExpressionWith<UnaryParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new BinaryExpression(unaryExpression, operatorToken, rightExpression);
            }

            return unaryExpression;
        }
    }
}