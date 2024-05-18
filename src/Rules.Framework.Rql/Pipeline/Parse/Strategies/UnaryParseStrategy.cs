namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class UnaryParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public UnaryParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.MINUS))
            {
                var @operator = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();
                var right = this.ParseExpressionWith<UnaryParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new UnaryExpression(@operator, right);
            }

            return this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
        }
    }
}