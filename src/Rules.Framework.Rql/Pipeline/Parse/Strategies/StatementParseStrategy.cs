namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class StatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public StatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            // TODO: future logic to be added here for dealing with if, foreach, and block statements.

            return this.ParseStatementWith<ExpressionStatementParseStrategy>(parseContext);
        }
    }
}