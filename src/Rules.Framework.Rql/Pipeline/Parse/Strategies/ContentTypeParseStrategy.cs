namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Linq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ContentTypeParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        private static readonly LiteralType[] allowedLiteralTypesAsContentType = new[] { LiteralType.Integer, LiteralType.String };

        private static readonly Lazy<string> allowedLiteralTypesMessage = new(() =>
            $"Only literals of types [{allowedLiteralTypesAsContentType.Select(t => t.ToString()).Aggregate((t1, t2) => $"{t1}, {t2}")}] are allowed.");

        public ContentTypeParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.FOR))
            {
                throw new InvalidOperationException("Unable to handle content type expression.");
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected content type name.", parseContext.GetNextToken());
                return Expression.None;
            }

            var contentExpression = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (contentExpression is LiteralExpression literalExpression && !allowedLiteralTypesAsContentType.Contains(literalExpression.Type))
            {
                parseContext.EnterPanicMode($"Literal '{literalExpression.Token.Lexeme}' is not allowed as a valid content type. {allowedLiteralTypesMessage.Value}", literalExpression.Token);
                return Expression.None;
            }

            return contentExpression;
        }
    }
}