namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Statements;

    internal class ProgrammableSubLanguageParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public ProgrammableSubLanguageParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            var expression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            return new ProgrammableSubLanguageStatement(expression);
        }
    }
}