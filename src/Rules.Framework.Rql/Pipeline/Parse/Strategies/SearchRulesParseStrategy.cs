namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class SearchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public SearchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                throw new InvalidOperationException("Unable to handle search rules expression.");
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.RULES))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.RULES)}'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.FOR)}'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SINCE))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.SINCE)}'.", parseContext.GetNextToken());
                return Expression.None;
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var dateBegin = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (dateBegin is LiteralExpression dateBeginLiteralExpression && dateBeginLiteralExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", dateBeginLiteralExpression.Token);
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.UNTIL))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.UNTIL)}'.", parseContext.GetNextToken());
                return Expression.None;
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var dateEnd = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (dateEnd is LiteralExpression dateEndLiteralExpression && dateEndLiteralExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", dateEndLiteralExpression.Token);
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

            return new SearchExpression(contentType, dateBegin, dateEnd, inputConditionsExpression);
        }
    }
}