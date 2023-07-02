namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class MatchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public MatchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.MATCH))
            {
                throw new InvalidOperationException("Unable to handle match rules expression.");
            }

            var cardinality = this.ParseExpressionWith<CardinalityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var matchDate = this.ParseDate(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            Expression inputConditionsExpression;
            if (parseContext.MoveNextIfNextToken(TokenType.WITH))
            {
                inputConditionsExpression = this.ParseExpressionWith<InputConditionsParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }
            }
            else
            {
                inputConditionsExpression = Expression.None;
            }

            return new MatchExpression(cardinality, contentType, matchDate, inputConditionsExpression);
        }

        private Expression ParseDate(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.ON))
            {
                parseContext.EnterPanicMode("Expect token 'ON'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            _ = parseContext.MoveNext();
            var matchDate = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return matchDate;
        }
    }
}