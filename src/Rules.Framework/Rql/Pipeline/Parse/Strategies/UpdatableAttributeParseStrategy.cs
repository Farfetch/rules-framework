namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class UpdatableAttributeParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public UpdatableAttributeParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.SET))
            {
                throw new InvalidOperationException("Unable to handle updatable attribute expression.");
            }

            if (parseContext.MoveNextIfNextToken(TokenType.ENDS))
            {
                var dateEnd = this.ParseSegmentWith<UpdatableDateEndParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Segment.None;
                }

                return new UpdatableAttributeSegment(dateEnd, UpdatableAttributeKind.DateEnd);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.PRIORITY))
            {
                var priorityOption = this.ParseSegmentWith<PriorityOptionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Segment.None;
                }

                return new UpdatableAttributeSegment(priorityOption, UpdatableAttributeKind.PriorityOption);
            }

            parseContext.EnterPanicMode("Expected updatable attribute (ENDS ON <date end>, PRIORITY NUMBER <priority value>).", parseContext.GetNextToken());
            return Segment.None;
        }
    }
}