namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionGroupingParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public ConditionGroupingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfCurrentToken(TokenType.BRACKET_LEFT))
            {
                var condition = this.ParseSegmentWith<ConditionGroupingParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Segment.None;
                }

                if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
                {
                    parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetCurrentToken());
                    return Segment.None;
                }

                return new ConditionGroupingSegment(condition);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                return this.ParseSegmentWith<ValueConditionParseStrategy>(parseContext);
            }

            return this.ParseSegmentWith<ComposedConditionParseStrategy>(parseContext);
        }
    }
}