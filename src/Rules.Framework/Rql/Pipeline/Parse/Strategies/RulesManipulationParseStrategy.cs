namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class RulesManipulationParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public RulesManipulationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.CREATE))
            {
                return this.ParseExpressionWith<CreateRuleParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.UPDATE))
            {
                return this.ParseExpressionWith<UpdateRuleParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ACTIVATE))
            {
                return this.ParseExpressionWith<ActivationParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.DEACTIVATE))
            {
                return this.ParseExpressionWith<DeactivationParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                return this.ParseExpressionWith<MatchRulesParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                return this.ParseExpressionWith<SearchRulesParseStrategy>(parseContext);
            }

            return this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
        }
    }
}