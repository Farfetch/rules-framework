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
            // TODO: future logic to be added here for dealing with variables.

            return this.ParseStatementWith<StatementParseStrategy>(parseContext);
        }
    }
}