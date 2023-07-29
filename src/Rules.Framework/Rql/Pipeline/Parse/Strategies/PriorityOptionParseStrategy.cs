namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class PriorityOptionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public PriorityOptionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.PRIORITY))
            {
                throw new InvalidOperationException("Unable to handle priority option expression.");
            }

            if (parseContext.MoveNextIfNextToken(TokenType.TOP, TokenType.BOTTOM))
            {
                return ParseTopOrBottom(parseContext);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.RULE))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.NAME))
                {
                    parseContext.EnterPanicMode("Expected token 'NAME'.", parseContext.GetCurrentToken());
                    return Segment.None;
                }

                return ParseRuleName(parseContext);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.NUMBER))
            {
                return ParsePriorityNumber(parseContext);
            }

            parseContext.EnterPanicMode("Expect one priority option (TOP, BOTTOM, RULE NAME <name>, or NUMBER <priority value>.", parseContext.GetCurrentToken());
            return Segment.None;
        }

        private Segment ParsePriorityNumber(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNextIfNextToken(TokenType.INT))
            {
                parseContext.EnterPanicMode("Expected priority value.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var priorityValue = this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new PriorityOptionSegment(keyword, priorityValue);
        }

        private Segment ParseRuleName(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNextIfNextToken(TokenType.STRING))
            {
                parseContext.EnterPanicMode("Expected rule name.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var ruleName = this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new PriorityOptionSegment(keyword, ruleName);
        }

        private Segment ParseTopOrBottom(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new PriorityOptionSegment(keyword, argument: null);
        }
    }
}