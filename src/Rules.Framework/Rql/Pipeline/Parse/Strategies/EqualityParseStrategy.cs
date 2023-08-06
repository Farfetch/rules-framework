namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class EqualityParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public EqualityParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var unaryExpression = this.ParseExpressionWith<ComparisonParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.EQUAL, TokenType.NOT_EQUAL))
            {
                var operatorSegment = this.ParseSegmentWith<OperatorParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                _ = parseContext.MoveNext();
                var rightExpression = this.ParseExpressionWith<ComparisonParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new BinaryExpression(unaryExpression, operatorSegment, rightExpression);
            }

            return unaryExpression;
        }
    }
}