namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Expressions;
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
                    parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetNextToken());
                    return Segment.None;
                }

                return new ConditionGroupingSegment(condition);
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                parseContext.EnterPanicMode("Expected token placeholder beginning with '@'", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var valueCondition = this.ParseSegmentWith<ValueConditionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            if (parseContext.IsMatchNextToken(TokenType.AND, TokenType.OR))
            {
                return this.ParseComposedCondition(parseContext, valueCondition);
            }

            return valueCondition;
        }

        private Segment ParseComposedCondition(ParseContext parseContext, Segment firstConditionSegment)
        {
            TokenType logicalOperatorTokenType = TokenType.None;
            Expression logicalOperator = null!;
            var childConditions = new List<Segment> { firstConditionSegment };
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