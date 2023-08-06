namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class NothingParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public NothingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.NOTHING))
            {
                return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            }

            return this.ParseExpressionWith<ComparisonParseStrategy>(parseContext);
        }
    }
}