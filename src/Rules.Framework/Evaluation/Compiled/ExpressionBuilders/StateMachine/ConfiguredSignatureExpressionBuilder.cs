namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;

    internal sealed class ConfiguredSignatureExpressionBuilder : IConfiguredSignatureExpressionBuilder
    {
        private readonly ExpressionConfiguration expressionConfiguration;

        public ConfiguredSignatureExpressionBuilder(ExpressionConfiguration expressionConfiguration)
        {
            this.expressionConfiguration = expressionConfiguration;
        }

        public IConfiguredExpressionBuilder SetImplementation(
            Action<IImplementationExpressionBuilder> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var implementationExpressionBuilder = new ImplementationExpressionBuilder(
                scopeName: string.Empty,
                parent: null,
                expressionConfiguration: this.expressionConfiguration);
            builder.Invoke(implementationExpressionBuilder);

            this.expressionConfiguration.LabelTargets = implementationExpressionBuilder.LabelTargets;
            this.expressionConfiguration.Variables = implementationExpressionBuilder.Variables;
            this.expressionConfiguration.Expressions = implementationExpressionBuilder.Expressions;

            return new ConfiguredExpressionBuilder(expressionConfiguration);
        }
    }
}