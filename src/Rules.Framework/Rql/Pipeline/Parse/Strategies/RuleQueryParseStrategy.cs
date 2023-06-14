namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class RuleQueryParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public RuleQueryParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            var queryExpression = ParseQueryExpression(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            return new RuleQueryStatement(queryExpression);
        }

        private Expression ParseQueryExpression(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                return this.ParseExpressionWith<MatchRulesParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                return this.ParseExpressionWith<SearchRulesParseStrategy>(parseContext);
            }

            throw new InvalidOperationException("Unable to handle rule query statement.");
        }
    }
}