namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ContentTypeParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ContentTypeParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                if (parseContext.MoveNextIfNextToken(TokenType.STRING))
                {
                    var contentType = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return contentType;
                }

                if (parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
                {
                    // TODO: future support.
                }

                parseContext.EnterPanicMode("Expected content type name.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}