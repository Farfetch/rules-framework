namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
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

            if (parseContext.IsMatchCurrentToken(TokenType.STRING, TokenType.INT, TokenType.BOOL, TokenType.DECIMAL, TokenType.DATE))
            {
                return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            }

            parseContext.EnterPanicMode("Expected expression.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}