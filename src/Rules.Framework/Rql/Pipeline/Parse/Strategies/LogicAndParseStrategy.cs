namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class LogicAndParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public LogicAndParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var unaryExpression = this.ParseExpressionWith<EqualityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.AND))
            {
                var operatorToken = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();
                var rightExpression = this.ParseExpressionWith<EqualityParseStrategy>(parseContext);
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