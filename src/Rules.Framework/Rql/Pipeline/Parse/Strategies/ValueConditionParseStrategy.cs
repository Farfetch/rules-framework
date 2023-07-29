namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class ValueConditionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public ValueConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                throw new InvalidOperationException("Unable to handle value condition expression.");
            }

            var leftToken = parseContext.GetCurrentToken();
            var leftExpression = new PlaceholderExpression(leftToken);

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected binary operator.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var operatorExpression = this.ParseSegmentWith<OperatorParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.INT, TokenType.STRING, TokenType.BOOL, TokenType.DECIMAL, TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected value for condition.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var rightExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new ValueConditionSegment(leftExpression, operatorExpression, rightExpression);
        }
    }
}