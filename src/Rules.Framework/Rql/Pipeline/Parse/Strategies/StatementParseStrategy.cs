namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class StatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public StatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                var matchExpression = this.ParseExpressionWith<MatchRulesParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new QueryStatement(matchExpression);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                var searchExpression = this.ParseExpressionWith<SearchRulesParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new QueryStatement(searchExpression);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.CREATE))
            {
                var createStatement = this.ParseStatementWith<CreateRuleParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new DefinitionStatement(createStatement);
            }

            _ = parseContext.MoveNext();
            parseContext.EnterPanicMode("Expected statement begin (MATCH, SEARCH).", parseContext.GetCurrentToken());
            return Statement.None;
        }
    }
}