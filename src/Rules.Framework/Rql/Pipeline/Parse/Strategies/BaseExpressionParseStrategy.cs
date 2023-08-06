namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class BaseExpressionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public BaseExpressionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.IDENTIFIER))
            {
                return this.ParseExpressionWith<IndexerParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ARRAY, TokenType.BRACE_LEFT))
            {
                return this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.OBJECT))
            {
                return this.ParseExpressionWith<ObjectParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.NOTHING))
            {
                return this.ParseExpressionWith<NothingParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.STRING, TokenType.INT, TokenType.BOOL, TokenType.DECIMAL, TokenType.DATE))
            {
                return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            }

            parseContext.EnterPanicMode("Expected expression.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}