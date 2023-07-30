namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class DeclarationParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public DeclarationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.VAR))
            {
                return this.ParseStatementWith<VariableDeclarationParseStrategy>(parseContext);
            }

            var statement = this.ParseStatementWith<StatementParseStrategy>(parseContext);
            if (statement == Statement.None && !parseContext.PanicMode)
            {
                var currentToken = parseContext.GetCurrentToken();
                parseContext.EnterPanicMode($"Unrecognized token '{currentToken.Lexeme}'.", currentToken);
                _ = parseContext.MoveNext();
                return Statement.None;
            }

            return statement;
        }
    }
}