namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class ComposedConditionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public ComposedConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            var conditionGrouping = this.ParseSegmentWith<ConditionGroupingParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            if (!parseContext.IsMatchNextToken(TokenType.AND, TokenType.OR))
            {
                parseContext.EnterPanicMode("Expected logical operator ('AND' or 'OR').", parseContext.GetCurrentToken());
                return Segment.None;
            }

            TokenType logicalOperatorTokenType = TokenType.None;
            Expression logicalOperator = null;
            var childConditions = new List<Segment> { conditionGrouping };
            while (parseContext.MoveNextIfNextToken(TokenType.AND, TokenType.OR))
            {
                if (logicalOperatorTokenType == TokenType.None)
                {
                    logicalOperatorTokenType = parseContext.GetCurrentToken().Type;
                    logicalOperator = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Segment.None;
                    }
                }
                else if (!parseContext.IsMatchCurrentToken(logicalOperatorTokenType))
                {
                    parseContext.EnterPanicMode("Mixup of logical operators ('AND' + 'OR') under the same condition grouping is not supported.", parseContext.GetCurrentToken());
                    return Segment.None;
                }

                if (!parseContext.MoveNext())
                {
                    parseContext.EnterPanicMode("Expected condition after logical operator .", parseContext.GetCurrentToken());
                    return Segment.None;
                }

                var nextCondition = this.ParseSegmentWith<ConditionGroupingParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Segment.None;
                }

                childConditions.Add(nextCondition);
            }

            return new ComposedConditionSegment(logicalOperator, childConditions.ToArray());
        }
    }
}