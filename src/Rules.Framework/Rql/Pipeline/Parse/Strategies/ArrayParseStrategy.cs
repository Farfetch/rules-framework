namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ArrayParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ArrayParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.ARRAY))
            {
                var arrayToken = parseContext.GetCurrentToken();
                if (parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_LEFT))
                {
                    var initializerBeginToken = parseContext.GetCurrentToken();
                    _ = parseContext.MoveNext();
                    var size = this.ParseSizeExpression(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_RIGHT))
                    {
                        parseContext.EnterPanicMode("Expected token ']'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    var initializerEndToken = parseContext.GetCurrentToken();
                    return new NewArrayExpression(arrayToken, initializerBeginToken, size, Array.Empty<Expression>(), initializerEndToken);
                }

                if (parseContext.MoveNextIfNextToken(TokenType.BRACE_LEFT))
                {
                    var initializerBeginToken = parseContext.GetCurrentToken();
                    _ = parseContext.MoveNext();
                    var literal = this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    _ = parseContext.MoveNext();
                    var values = new List<Expression> { literal };
                    while (parseContext.MoveNextIfCurrentToken(TokenType.COMMA))
                    {
                        literal = this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
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

                    var initializerEndToken = parseContext.GetCurrentToken();
                    return new NewArrayExpression(arrayToken, initializerBeginToken, Expression.None, values.ToArray(), initializerEndToken);
                }
            }

            return this.ParseExpressionWith<ObjectParseStrategy>(parseContext);
        }

        private Expression ParseSizeExpression(ParseContext parseContext)
        {
            var literal = this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (literal is LiteralExpression literalExpression && literalExpression.Type != LiteralType.Integer)
            {
                parseContext.EnterPanicMode("Expected integer literal.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return literal;
        }
    }
}