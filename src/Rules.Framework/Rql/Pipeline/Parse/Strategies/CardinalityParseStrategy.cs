namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class CardinalityParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public CardinalityParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.ONE))
            {
                var oneCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULE))
                {
                    parseContext.EnterPanicMode("Expected token 'RULE'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                return new CardinalityExpression(oneCardinalityKeyword, ruleKeyword);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ALL))
            {
                var allCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULES))
                {
                    parseContext.EnterPanicMode("Expected token 'RULES'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                return new CardinalityExpression(allCardinalityKeyword, ruleKeyword);
            }

            parseContext.EnterPanicMode("Expected tokens 'ONE' or 'ALL'.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}