namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class CardinalityParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public CardinalityParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNext())
            {
                if (parseContext.IsMatchCurrentToken(TokenType.ONE))
                {
                    var oneCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                    if (parseContext.MoveNext() && !parseContext.IsMatchCurrentToken(TokenType.RULE))
                    {
                        parseContext.EnterPanicMode("Expect token 'RULE'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                    return new CardinalityExpression(oneCardinalityKeyword, ruleKeyword);
                }

                if (parseContext.IsMatchCurrentToken(TokenType.ALL))
                {
                    var allCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                    if (parseContext.MoveNext() && !parseContext.IsMatchCurrentToken(TokenType.RULES))
                    {
                        parseContext.EnterPanicMode("Expect token 'RULES'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                    return new CardinalityExpression(allCardinalityKeyword, ruleKeyword);
                }
            }

            parseContext.EnterPanicMode("Expect tokens 'ONE' or 'ALL'.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}