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
            if (!parseContext.MoveNextIfCurrentToken(TokenType.SEARCH))
            {
                throw new InvalidOperationException("Unable to handle search rules expression.");
            }

            if (!parseContext.MoveNextIfCurrentToken(TokenType.RULES))
            {
                parseContext.EnterPanicMode("Expected token 'RULES'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BEGINS))
            {
                parseContext.EnterPanicMode("Expected token 'STARTS'.", parseContext.GetNextToken());
                return Expression.None;
            }

            var dateBegin = this.ParseExpressionWith<DateBeginParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.ENDS))
            {
                parseContext.EnterPanicMode("Expected token 'ENDS'.", parseContext.GetNextToken());
                return Expression.None;
            }

            var dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
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
                if (!parseContext.IsMatchNextToken(TokenType.SEMICOLON))
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