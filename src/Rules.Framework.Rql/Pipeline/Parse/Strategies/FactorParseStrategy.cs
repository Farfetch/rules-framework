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

            if (parseContext.MoveNextIfNextToken(TokenType.SLASH, TokenType.STAR))
            {
                var rightExpression = Expression.None;
                var operatorSegment = this.ParseSegmentWith<OperatorParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return new BinaryExpression(unaryExpression, operatorSegment, rightExpression);
                }

                _ = parseContext.MoveNext();
                rightExpression = this.ParseExpressionWith<UnaryParseStrategy>(parseContext);
                return new BinaryExpression(unaryExpression, operatorSegment, rightExpression);
            }

            return unaryExpression;
        }
    }
}