namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
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
            var cardinality = this.ParseExpressionWith<CardinalityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
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

            var inputConditionExpressions = this.ParseExpressionWith<InputConditionsParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new MatchExpression(cardinality, contentType, matchDate, inputConditionExpressions);
        }

        private Expression ParseDate(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.MoveNextIfCurrentToken(TokenType.ON))
            {
                if (parseContext.IsMatchCurrentToken(TokenType.STRING))
                {
                    var matchDate = this.ParseExpressionWith<DateTimeLiteralParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return matchDate;
                }

                parseContext.EnterPanicMode("Expect match date and time.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            parseContext.EnterPanicMode("Expect token 'ON'.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}