namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class NamedExpressionBuilder : INamedExpressionBuilder
    {
        private readonly ExpressionConfiguration expressionConfiguration;

        public NamedExpressionBuilder(ExpressionConfiguration expressionConfiguration)
        {
            this.expressionConfiguration = expressionConfiguration;
        }

        public IParameterizedExpressionBuilder WithoutParameters()
        {
            this.expressionConfiguration.Parameters = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);

            return new ParameterizedExpressionBuilder(this.expressionConfiguration);
        }

        public IParameterizedExpressionBuilder WithParameters(
            Action<IExpressionParametersConfiguration> parametersConfigurationAction)
        {
            if (parametersConfigurationAction is null)
            {
                throw new ArgumentNullException(nameof(parametersConfigurationAction));
            }

            var expressionBuilderParametersConfiguration = new ExpressionParametersConfiguration();
            parametersConfigurationAction.Invoke(expressionBuilderParametersConfiguration);

            this.expressionConfiguration.Parameters = expressionBuilderParametersConfiguration.Parameters;

            return new ParameterizedExpressionBuilder(this.expressionConfiguration);
        }
    }
}