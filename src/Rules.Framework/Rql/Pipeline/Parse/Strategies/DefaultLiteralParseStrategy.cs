namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class DefaultLiteralParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public DefaultLiteralParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var literalToken = parseContext.GetCurrentToken();
            var inferredLiteralType = literalToken.Type switch
            {
                TokenType.INT => LiteralType.Integer,
                TokenType.BOOL => LiteralType.Bool,
                TokenType.DECIMAL => LiteralType.Decimal,
                TokenType.STRING => LiteralType.String,
                TokenType.NOTHING => LiteralType.Undefined,
                _ => throw new NotSupportedException($"The token type '{literalToken.Type}' is not supported as a valid literal type."),
            };
            return new LiteralExpression(inferredLiteralType, literalToken, literalToken.Literal);
        }
    }
}