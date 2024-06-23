namespace Rules.Framework.Rql.Pipeline.Parse
{
    internal interface IParseStrategyProvider
    {
        TExpressionParseStrategy GetExpressionParseStrategy<TExpressionParseStrategy>() where TExpressionParseStrategy : IExpressionParseStrategy;

        TSegmentParseStrategy GetSegmentParseStrategy<TSegmentParseStrategy>() where TSegmentParseStrategy : ISegmentParseStrategy;

        TStatementParseStrategy GetStatementParseStrategy<TStatementParseStrategy>() where TStatementParseStrategy : IStatementParseStrategy;
    }
}