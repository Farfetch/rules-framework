namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;

    internal abstract class ParseStrategyBase<TParseOutput> : IParseStrategy<TParseOutput>
    {
        private readonly IParseStrategyProvider parseStrategyProvider;

        protected ParseStrategyBase(IParseStrategyProvider parseStrategyProvider)
        {
            this.parseStrategyProvider = parseStrategyProvider;
        }

        public abstract TParseOutput Parse(ParseContext parseContext);

        protected Expression ParseExpressionWith<TExpressionParseStrategy>(ParseContext parseContext) where TExpressionParseStrategy : IExpressionParseStrategy
            => this.parseStrategyProvider.GetExpressionParseStrategy<TExpressionParseStrategy>().Parse(parseContext);

        protected Statement ParseStatementWith<TStatementParseStrategy>(ParseContext parseContext) where TStatementParseStrategy : IStatementParseStrategy
            => this.parseStrategyProvider.GetStatementParseStrategy<TStatementParseStrategy>().Parse(parseContext);
    }
}