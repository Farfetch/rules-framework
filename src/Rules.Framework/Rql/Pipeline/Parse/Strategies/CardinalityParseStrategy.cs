namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
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
            if (parseContext.IsMatchCurrentToken(TokenType.ONE))
            {
                var oneCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULE))
                {
                    parseContext.EnterPanicMode("Expected token 'RULE'.", parseContext.GetNextToken());
                    return Segment.None;
                }

                var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                return new CardinalitySegment(oneCardinalityKeyword, ruleKeyword);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ALL))
            {
                var allCardinalityKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                if (!parseContext.MoveNextIfNextToken(TokenType.RULES))
                {
                    parseContext.EnterPanicMode("Expected token 'RULES'.", parseContext.GetNextToken());
                    return Segment.None;
                }

                var ruleKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);

                return new CardinalitySegment(allCardinalityKeyword, ruleKeyword);
            }

            parseContext.EnterPanicMode("Expected tokens 'ONE' or 'ALL'.", parseContext.GetCurrentToken());
            return Segment.None;
        }
    }
}