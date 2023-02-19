namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal class ExpressionBuilder : IExpressionParametersBuilder, IExpressionReturnBuilder, IExpressionImplementationBuilder, IConfiguredExpressionBuilder
    {
        private readonly IExpressionBuilderFactory factory;

        public ExpressionBuilder(ExpressionConfiguration expressionConfiguration, IExpressionBuilderFactory factory)
        {
            this.ExpressionConfiguration = expressionConfiguration;
            this.factory = factory;
        }

        public static IExpressionBuilderFactory ExpressionBuilderFactory { get; set; } = new DefaultExpressionBuilderFactory();

        public ExpressionConfiguration ExpressionConfiguration { get; }

        public static IExpressionParametersBuilder NewExpression(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A non-null, empty, or white-space expression name must be provided.", nameof(name));
            }

            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = name,
            };

            return ExpressionBuilderFactory.CreateExpressionBuilder(expressionConfiguration);
        }

        public ExpressionResult Build()
        {
            var variableExpressionsCopy = new List<ParameterExpression>(this.ExpressionConfiguration.Variables.Values);
            var bodyBlockExpressionsCopy = new List<Expression>(this.ExpressionConfiguration.Expressions)
            {
                Expression.Label(
                    this.ExpressionConfiguration.ReturnLabelTarget,
                    Expression.Constant(this.ExpressionConfiguration.ReturnDefaultValue)),
            };

            var implementationExpression = Expression.Block(variables: variableExpressionsCopy, expressions: bodyBlockExpressionsCopy);

            return new ExpressionResult(
                this.ExpressionConfiguration.ExpressionName,
                implementationExpression,
                this.ExpressionConfiguration.Parameters.Values,
                this.ExpressionConfiguration.ReturnType);
        }

        public IExpressionImplementationBuilder HavingReturn(Type type, object defaultValue)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.ExpressionConfiguration.ReturnType = type;
            this.ExpressionConfiguration.ReturnDefaultValue = defaultValue;
            this.ExpressionConfiguration.ReturnLabelTarget
                = Expression.Label(type, $"{this.ExpressionConfiguration.ExpressionName}_ReturnLabel");

            return this;
        }

        public IExpressionImplementationBuilder HavingReturn<T>(object defaultValue)
            => this.HavingReturn(typeof(T), defaultValue);

        public IConfiguredExpressionBuilder SetImplementation(
            Action<IExpressionBlockBuilder> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var implementationExpressionBuilder = this.factory.CreateExpressionBlockBuilder(
                scopeName: string.Empty,
                parent: null,
                expressionConfiguration: this.ExpressionConfiguration);
            builder.Invoke(implementationExpressionBuilder);

            this.ExpressionConfiguration.LabelTargets = implementationExpressionBuilder.LabelTargets;
            this.ExpressionConfiguration.Variables = implementationExpressionBuilder.Variables;
            this.ExpressionConfiguration.Expressions = implementationExpressionBuilder.Expressions;

            return this;
        }

        public IExpressionReturnBuilder WithoutParameters()
        {
            this.ExpressionConfiguration.Parameters = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);

            return this;
        }

        public IExpressionReturnBuilder WithParameters(
            Action<IExpressionParametersConfiguration> parametersConfigurationAction)
        {
            if (parametersConfigurationAction is null)
            {
                throw new ArgumentNullException(nameof(parametersConfigurationAction));
            }

            var expressionBuilderParametersConfiguration = this.factory.CreateExpressionParametersConfiguration();
            parametersConfigurationAction.Invoke(expressionBuilderParametersConfiguration);

            this.ExpressionConfiguration.Parameters = expressionBuilderParametersConfiguration.Parameters;

            return this;
        }
    }
}