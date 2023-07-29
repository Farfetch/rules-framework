namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionsDefinitionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public ConditionsDefinitionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.APPLY))
            {
                throw new InvalidOperationException("Unable to handle conditions definition expression.");
            }

            if (!parseContext.MoveNextIfCurrentToken(TokenType.WHEN))
            {
                parseContext.EnterPanicMode("Expect token 'WHEN'.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            return this.ParseSegmentWith<ConditionGroupingParseStrategy>(parseContext);
        }
    }
}