namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ConfiguredExpressionBuilder : IConfiguredExpressionBuilder
    {
        private readonly ExpressionConfiguration expressionConfiguration;

        public ConfiguredExpressionBuilder(ExpressionConfiguration expressionConfiguration)
        {
            this.expressionConfiguration = expressionConfiguration;
        }

        public ExpressionResult Build()
        {
            var variableExpressionsCopy = new List<ParameterExpression>(this.expressionConfiguration.Variables.Values);
            var bodyBlockExpressionsCopy = new List<Expression>(this.expressionConfiguration.Expressions)
            {
                Expression.Label(
                    this.expressionConfiguration.ReturnLabelTarget,
                    Expression.Constant(this.expressionConfiguration.ReturnDefaultValue)),
            };

            var implementationExpression = Expression.Block(variables: variableExpressionsCopy, expressions: bodyBlockExpressionsCopy);

            return new ExpressionResult(
                this.expressionConfiguration.ExpressionName,
                implementationExpression,
                this.expressionConfiguration.Parameters.Values,
                this.expressionConfiguration.ReturnType);
        }
    }
}