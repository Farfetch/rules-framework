namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class LogicOrParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public LogicOrParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var unaryExpression = this.ParseExpressionWith<LogicAndParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.OR))
            {
                var operatorToken = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();
                var rightExpression = this.ParseExpressionWith<LogicAndParseStrategy>(parseContext);
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