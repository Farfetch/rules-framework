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
            var statement = this.ParseStatement(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SEMICOLON))
            {
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetCurrentToken());
                return Statement.None;
            }

            return statement;
        }

        private Statement ParseStatement(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.MATCH, TokenType.SEARCH))
            {
                return this.ParseStatementWith<RuleQueryParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.CREATE, TokenType.UPDATE, TokenType.ACTIVATE, TokenType.DEACTIVATE))
            {
                return this.ParseStatementWith<RuleDefinitionParseStrategy>(parseContext);
            }

            return this.ParseStatementWith<ProgrammableSubLanguageParseStrategy>(parseContext);
        }
    }
}