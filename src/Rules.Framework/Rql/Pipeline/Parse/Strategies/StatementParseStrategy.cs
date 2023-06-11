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

            if (parseContext.IsMatchCurrentToken(TokenType.UPDATE))
            {
                var updateStatement = this.ParseStatementWith<UpdateRuleParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new DefinitionStatement(updateStatement);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ACTIVATE))
            {
                var activationStatement = this.ParseStatementWith<ActivationParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new DefinitionStatement(activationStatement);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.DEACTIVATE))
            {
                var deactivationStatement = this.ParseStatementWith<DeactivationParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new DefinitionStatement(deactivationStatement);
            }

            var currentToken = parseContext.GetCurrentToken();
            parseContext.EnterPanicMode($"Unrecognized token '{currentToken.Lexeme}'.", currentToken);
            _ = parseContext.MoveNext();
            return Statement.None;
        }
    }
}