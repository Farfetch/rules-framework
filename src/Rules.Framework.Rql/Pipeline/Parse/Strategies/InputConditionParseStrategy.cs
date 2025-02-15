namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class InputConditionParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public InputConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                throw new InvalidOperationException("Unable to handle input condition expression.");
            }

            var leftToken = parseContext.GetCurrentToken();
            var leftExpression = new PlaceholderExpression(leftToken);
            var operatorToken = Token.None;
            var rightExpression = Expression.None;

            if (!parseContext.MoveNextIfNextToken(TokenType.IS))
            {
                parseContext.EnterPanicMode("Expected token 'IS'.", parseContext.GetCurrentToken());
                return new InputConditionSegment(leftExpression, operatorToken, rightExpression);
            }

            operatorToken = parseContext.GetCurrentToken();

            if (parseContext.MoveNextIfNextToken(TokenType.STRING, TokenType.INT, TokenType.DECIMAL, TokenType.BOOL, TokenType.IDENTIFIER))
            {
                rightExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
                return new InputConditionSegment(leftExpression, operatorToken, rightExpression);
            }

            parseContext.EnterPanicMode("Expected literal for condition.", parseContext.GetNextToken());
            return new InputConditionSegment(leftExpression, operatorToken, rightExpression);
        }
    }
}