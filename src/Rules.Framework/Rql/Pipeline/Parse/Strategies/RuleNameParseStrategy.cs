namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class RuleNameParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public RuleNameParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.RULE))
            {
                parseContext.EnterPanicMode("Expected token 'RULE'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.STRING))
            {
                parseContext.EnterPanicMode("Expected rule name.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
        }
    }
}