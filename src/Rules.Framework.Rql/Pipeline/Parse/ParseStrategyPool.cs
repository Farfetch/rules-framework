namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Collections.Generic;

    internal class ParseStrategyPool : IParseStrategyProvider
    {
        private readonly Dictionary<Type, IExpressionParseStrategy> expressionParseStrategies;
        private readonly Dictionary<Type, ISegmentParseStrategy> segmentParseStrategies;
        private readonly Dictionary<Type, IStatementParseStrategy> statementParseStrategies;

        public ParseStrategyPool()
        {
            this.expressionParseStrategies = new Dictionary<Type, IExpressionParseStrategy>();
            this.segmentParseStrategies = new Dictionary<Type, ISegmentParseStrategy>();
            this.statementParseStrategies = new Dictionary<Type, IStatementParseStrategy>();
        }

        public TExpressionParseStrategy GetExpressionParseStrategy<TExpressionParseStrategy>() where TExpressionParseStrategy : IExpressionParseStrategy
        {
            if (this.expressionParseStrategies.TryGetValue(typeof(TExpressionParseStrategy), out var expressionParseStrategy))
            {
                return (TExpressionParseStrategy)expressionParseStrategy;
            }

            expressionParseStrategy = (TExpressionParseStrategy)Activator.CreateInstance(typeof(TExpressionParseStrategy), this);
            this.expressionParseStrategies[typeof(TExpressionParseStrategy)] = expressionParseStrategy;
            return (TExpressionParseStrategy)expressionParseStrategy;
        }

        public TSegmentParseStrategy GetSegmentParseStrategy<TSegmentParseStrategy>() where TSegmentParseStrategy : ISegmentParseStrategy
        {
            if (this.segmentParseStrategies.TryGetValue(typeof(TSegmentParseStrategy), out var segmentParseStrategy))
            {
                return (TSegmentParseStrategy)segmentParseStrategy;
            }

            segmentParseStrategy = (TSegmentParseStrategy)Activator.CreateInstance(typeof(TSegmentParseStrategy), this);
            this.segmentParseStrategies[typeof(TSegmentParseStrategy)] = segmentParseStrategy;
            return (TSegmentParseStrategy)segmentParseStrategy;
        }

        public TStatementParseStrategy GetStatementParseStrategy<TStatementParseStrategy>() where TStatementParseStrategy : IStatementParseStrategy
        {
            if (this.statementParseStrategies.TryGetValue(typeof(TStatementParseStrategy), out var statementParseStrategy))
            {
                return (TStatementParseStrategy)statementParseStrategy;
            }

            statementParseStrategy = (TStatementParseStrategy)Activator.CreateInstance(typeof(TStatementParseStrategy), this);
            this.statementParseStrategies[typeof(TStatementParseStrategy)] = statementParseStrategy;
            return (TStatementParseStrategy)statementParseStrategy;
        }
    }
}