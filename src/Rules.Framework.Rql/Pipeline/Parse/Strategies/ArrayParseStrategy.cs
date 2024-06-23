namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ArrayParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ArrayParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.ARRAY, TokenType.BRACE_LEFT))
            {
                throw new InvalidOperationException("Unable to handle array expression.");
            }

            Token initializerBeginToken;
            Token initializerEndToken;
            if (parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                initializerBeginToken = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();

                // TODO: update according to future logic to process 'or' expressions.
                var literal = this.ParseExpressionWith<TermParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                _ = parseContext.MoveNext();
                var values = new List<Expression> { literal };
                while (parseContext.IsMatchCurrentToken(TokenType.COMMA))
                {
                    _ = parseContext.MoveNext();

                    // TODO: update according to future logic to process 'or' expressions.
                    literal = this.ParseExpressionWith<TermParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    values.Add(literal);
                    _ = parseContext.MoveNext();
                }

                if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_RIGHT))
                {
                    parseContext.EnterPanicMode("Expected token '}'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                initializerEndToken = parseContext.GetCurrentToken();
                return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
            }

            // At this moment, assumes that an empty with fixed size is being declared.
            var arrayToken = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_LEFT))
            {
                parseContext.EnterPanicMode("Expected token '['.", parseContext.GetNextToken());
                return Expression.None;
            }

            initializerBeginToken = parseContext.GetCurrentToken();
            _ = parseContext.MoveNext();
            var size = this.ParseSizeExpression(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ']'.", parseContext.GetNextToken());
                return Expression.None;
            }

            initializerEndToken = parseContext.GetCurrentToken();
            return NewArrayExpression.Create(arrayToken, initializerBeginToken, size, Array.Empty<Expression>(), initializerEndToken);
        }

        private Expression ParseSizeExpression(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.INT))
            {
                parseContext.EnterPanicMode("Expected integer literal.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var literal = this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return literal;
        }
    }
}