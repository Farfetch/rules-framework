namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
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
            if (parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                var leftToken = parseContext.GetCurrentToken();
                var leftExpression = new PlaceholderExpression(leftToken);

                if (!parseContext.MoveNextIfNextToken(TokenType.IS))
                {
                    parseContext.EnterPanicMode("Expected token 'IS'.", parseContext.GetCurrentToken());
                    return Segment.None;
                }

                var operatorToken = parseContext.GetCurrentToken();

                if (parseContext.MoveNextIfNextToken(TokenType.STRING, TokenType.INT, TokenType.DECIMAL, TokenType.BOOL, TokenType.IDENTIFIER))
                {
                    var rightExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Segment.None;
                    }

                    return new InputConditionSegment(leftExpression, operatorToken, rightExpression);
                }

                parseContext.EnterPanicMode("Expected literal for condition", parseContext.GetNextToken());
                return Segment.None;
            }

            parseContext.EnterPanicMode("Expected placeholder (@<placeholder name>) for condition", parseContext.GetCurrentToken());
            return Segment.None;
        }
    }
}