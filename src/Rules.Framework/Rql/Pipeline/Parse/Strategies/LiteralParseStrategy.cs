namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Globalization;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class LiteralParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public LiteralParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var literalToken = parseContext.GetCurrentToken();
            if (literalToken.Type == TokenType.DATE)
            {
                if (!DateTime.TryParse((string)literalToken.Literal, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTimeLiteral))
                {
                    parseContext.EnterPanicMode("Expected date token.", literalToken);
                    return Expression.None;
                }

                return new LiteralExpression(LiteralType.DateTime, literalToken, dateTimeLiteral);
            }

            var inferredLiteralType = literalToken.Type switch
            {
                TokenType.BOOL => LiteralType.Bool,
                TokenType.DECIMAL => LiteralType.Decimal,
                TokenType.INT => LiteralType.Integer,
                TokenType.NOTHING => LiteralType.Undefined,
                TokenType.STRING => LiteralType.String,
                _ => throw new NotSupportedException($"The token type '{literalToken.Type}' is not supported as a valid literal type."),
            };
            return new LiteralExpression(inferredLiteralType, literalToken, literalToken.Literal);
        }
    }
}