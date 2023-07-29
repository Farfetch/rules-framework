namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Segments;

    internal class UpdatableDateEndParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public UpdatableDateEndParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            var dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new DateEndSegment(dateEnd);
        }
    }
}