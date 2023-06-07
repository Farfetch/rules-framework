namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionsDefinitionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ConditionsDefinitionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.APPLY))
            {
                parseContext.EnterPanicMode("Expected token 'APPLY'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.WHEN))
            {
                parseContext.EnterPanicMode("Expect token 'WHEN'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
        }
    }
}