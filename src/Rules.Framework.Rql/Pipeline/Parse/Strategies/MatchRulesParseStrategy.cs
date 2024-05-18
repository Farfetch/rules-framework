namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class MatchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public MatchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                throw new InvalidOperationException("Unable to handle match rules expression.");
            }

            _ = parseContext.MoveNext();
            var cardinality = this.ParseSegmentWith<CardinalityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetNextToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var matchDate = this.ParseDate(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            Segment inputConditionsExpression;
            if (parseContext.MoveNextIfNextToken(TokenType.WHEN))
            {
                inputConditionsExpression = this.ParseSegmentWith<InputConditionsParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }
            }
            else
            {
                if (!parseContext.IsMatchNextToken(TokenType.SEMICOLON, TokenType.EOF))
                {
                    var token = parseContext.GetNextToken();
                    parseContext.EnterPanicMode($"Unrecognized token '{token.Lexeme}'.", token);
                    return Expression.None;
                }

                inputConditionsExpression = Segment.None;
            }

            return MatchExpression.Create(cardinality, contentType, matchDate, inputConditionsExpression);
        }

        private Expression ParseDate(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.ON))
            {
                parseContext.EnterPanicMode("Expected token 'ON'.", parseContext.GetNextToken());
                return Expression.None;
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var matchDate = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (matchDate is LiteralExpression literalExpression && literalExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return matchDate;
        }
    }
}