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

            var initializerBeginToken = Token.None;
            var initializerEndToken = Token.None;
            var size = Expression.None;
            if (parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                var values = new List<Expression>();
                initializerBeginToken = parseContext.GetCurrentToken();
                if (!parseContext.MoveNext())
                {
                    parseContext.EnterPanicMode("Expected values following array initialization token '{'.", parseContext.GetCurrentToken());
                    return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
                }

                // TODO: update according to future logic to process 'or' expressions.
                var literal = this.ParseExpressionWith<TermParseStrategy>(parseContext);
                values.Add(literal);
                if (parseContext.PanicMode)
                {
                    return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
                }

                _ = parseContext.MoveNext();
                while (parseContext.IsMatchCurrentToken(TokenType.COMMA))
                {
                    _ = parseContext.MoveNext();

                    // TODO: update according to future logic to process 'or' expressions.
                    literal = this.ParseExpressionWith<TermParseStrategy>(parseContext);
                    values.Add(literal);
                    if (parseContext.PanicMode)
                    {
                        return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
                    }

                    _ = parseContext.MoveNext();
                }

                if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_RIGHT))
                {
                    parseContext.EnterPanicMode("Expected token '}'.", parseContext.GetCurrentToken());
                    return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
                }

                initializerEndToken = parseContext.GetCurrentToken();
                return NewArrayExpression.Create(Token.None, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
            }

            // At this moment, assumes that an empty with fixed size is being declared.
            var arrayToken = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_LEFT))
            {
                parseContext.EnterPanicMode("Expected token '['.", parseContext.GetNextToken());
                return NewArrayExpression.Create(arrayToken, initializerBeginToken, size, Array.Empty<Expression>(), initializerEndToken);
            }

            initializerBeginToken = parseContext.GetCurrentToken();
            _ = parseContext.MoveNext();
            size = this.ParseSizeExpression(parseContext);
            if (parseContext.PanicMode)
            {
                return NewArrayExpression.Create(arrayToken, initializerBeginToken, size, Array.Empty<Expression>(), initializerEndToken);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ']'.", parseContext.GetNextToken());
                return NewArrayExpression.Create(arrayToken, initializerBeginToken, size, Array.Empty<Expression>(), initializerEndToken);
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