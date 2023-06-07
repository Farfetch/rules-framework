namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class SearchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public SearchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.IsMatchCurrentToken(TokenType.RULES))
            {
                var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                var dateBegin = this.ParseExpressionWith<DateBeginParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                var dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                var inputConditionExpressions = this.ParseExpressionWith<InputConditionsParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new SearchExpression(contentType, dateBegin, dateEnd, inputConditionExpressions);
            }

            parseContext.EnterPanicMode("Expect token 'RULES'.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}