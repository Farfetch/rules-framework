namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal class DefaultExpressionBuilderFactory : IExpressionBuilderFactory
    {
        public IExpressionBlockBuilder CreateExpressionBlockBuilder(
            string scopeName,
            IExpressionBlockBuilder parent,
            ExpressionConfiguration expressionConfiguration)
        {
            if (expressionConfiguration is null)
            {
                throw new ArgumentNullException(nameof(expressionConfiguration));
            }

            return new ExpressionBlockBuilder(scopeName, parent, this, expressionConfiguration);
        }

        public IExpressionParametersBuilder CreateExpressionBuilder(
            ExpressionConfiguration expressionConfiguration)
        {
            if (expressionConfiguration is null)
            {
                throw new ArgumentNullException(nameof(expressionConfiguration));
            }

            return new ExpressionBuilder(expressionConfiguration, this);
        }

        public IExpressionParametersConfiguration CreateExpressionParametersConfiguration()
        {
            return new ExpressionParametersConfiguration();
        }

        public IExpressionSwitchBuilder CreateExpressionSwitchBuilder(
            IExpressionBlockBuilder parent)
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            return new ExpressionSwitchBuilder(parent);
        }
    }
}