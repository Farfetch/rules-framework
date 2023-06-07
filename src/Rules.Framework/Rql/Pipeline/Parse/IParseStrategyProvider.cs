namespace Rules.Framework.Rql.Pipeline.Parse
{
    internal interface IParseStrategyProvider
    {
        TExpressionParseStrategy GetExpressionParseStrategy<TExpressionParseStrategy>() where TExpressionParseStrategy : IExpressionParseStrategy;

        TStatementParseStrategy GetStatementParseStrategy<TStatementParseStrategy>() where TStatementParseStrategy : IStatementParseStrategy;
    }
}