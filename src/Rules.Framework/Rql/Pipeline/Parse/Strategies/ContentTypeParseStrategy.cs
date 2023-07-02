namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ContentTypeParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ContentTypeParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.FOR))
            {
                throw new InvalidOperationException("Unable to handle content type expression.");
            }

            if (parseContext.MoveNextIfNextToken(TokenType.STRING))
            {
                var contentType = this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
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
    }
}