namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class CardinalityParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public CardinalityParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            var cardinalityKeyword = Expression.None;
            var ruleKeyword = Expression.None;
            if (parseContext.IsMatchCurrentToken(TokenType.ONE))
            {
                cardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULE))
                {
                    parseContext.EnterPanicMode("Expected token 'RULE'.", parseContext.GetNextToken());
                    return CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);
                }

                ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                return CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ALL))
            {
                cardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULES))
                {
                    parseContext.EnterPanicMode("Expected token 'RULES'.", parseContext.GetNextToken());
                    return CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);
                }

                ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                return CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);
            }

            parseContext.EnterPanicMode("Expected tokens 'ONE' or 'ALL'.", parseContext.GetCurrentToken());
            return CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);
        }
    }
}