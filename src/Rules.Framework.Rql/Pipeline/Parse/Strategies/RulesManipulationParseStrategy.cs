namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class RulesManipulationParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public RulesManipulationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            // TODO: future logic to be added here for dealing with create, update, activate, and deactivate rules.
            if (parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                return this.ParseExpressionWith<MatchRulesParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                return this.ParseExpressionWith<SearchRulesParseStrategy>(parseContext);
            }

            // TODO: update according to future logic to process 'or' expressions.
            return this.ParseExpressionWith<TermParseStrategy>(parseContext);
        }
    }
}