namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class ValueConditionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        private static readonly IEnumerable<TokenType> allowedOperatorTokenTypes = new[]
        {
            TokenType.EQUAL,
            TokenType.NOT_EQUAL,
            TokenType.GREATER_THAN,
            TokenType.GREATER_THAN_OR_EQUAL,
            TokenType.LESS_THAN,
            TokenType.LESS_THAN_OR_EQUAL,
            TokenType.IN,
            TokenType.NOT,
        };

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
                parseContext.EnterPanicMode("Expected binary operator.", parseContext.GetNextToken());
                return Segment.None;
            }

            var operatorSegment = this.ParseSegmentWith<OperatorParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            if (IsNotSupportedOperator((OperatorSegment)operatorSegment))
            {
                parseContext.EnterPanicMode("Operator not supported for condition.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.INT, TokenType.STRING, TokenType.BOOL, TokenType.DECIMAL, TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected value for condition.", parseContext.GetNextToken());
                return Segment.None;
            }

            var rightExpression = this.ParseExpressionWith<EqualityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            return new ValueConditionSegment(leftExpression, operatorSegment, rightExpression);
        }

        private static bool IsNotSupportedOperator(OperatorSegment operatorSegment)
        {
            foreach (var token in operatorSegment.Tokens)
            {
                if (!allowedOperatorTokenTypes.Contains(token.Type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}