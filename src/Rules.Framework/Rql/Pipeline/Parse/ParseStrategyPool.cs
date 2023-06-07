namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Collections.Generic;

    internal class ParseStrategyPool : IParseStrategyProvider
    {
        private readonly Dictionary<Type, IExpressionParseStrategy> expressionParseStrategies;
        private readonly Dictionary<Type, IStatementParseStrategy> statementParseStrategies;

        public ParseStrategyPool()
        {
            this.expressionParseStrategies = new Dictionary<Type, IExpressionParseStrategy>();
            this.statementParseStrategies = new Dictionary<Type, IStatementParseStrategy>();
        }

        public TExpressionParseStrategy GetExpressionParseStrategy<TExpressionParseStrategy>() where TExpressionParseStrategy : IExpressionParseStrategy
        {
            if (this.expressionParseStrategies.TryGetValue(typeof(TExpressionParseStrategy), out var statementParseStrategy))
            {
                return (TExpressionParseStrategy)statementParseStrategy;
            }

            statementParseStrategy = (TExpressionParseStrategy)Activator.CreateInstance(typeof(TExpressionParseStrategy), this);
            this.expressionParseStrategies[typeof(TExpressionParseStrategy)] = statementParseStrategy;
            return (TExpressionParseStrategy)statementParseStrategy;
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