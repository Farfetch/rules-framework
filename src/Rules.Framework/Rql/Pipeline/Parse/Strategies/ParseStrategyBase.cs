namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;

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

        protected Segment ParseSegmentWith<TSegmentParseStrategy>(ParseContext parseContext) where TSegmentParseStrategy : ISegmentParseStrategy
            => this.parseStrategyProvider.GetSegmentParseStrategy<TSegmentParseStrategy>().Parse(parseContext);

        protected Statement ParseStatementWith<TStatementParseStrategy>(ParseContext parseContext) where TStatementParseStrategy : IStatementParseStrategy
            => this.parseStrategyProvider.GetStatementParseStrategy<TStatementParseStrategy>().Parse(parseContext);
    }
}