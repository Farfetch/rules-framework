namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Linq.Expressions;

    internal sealed class ParameterizedExpressionBuilder : IParameterizedExpressionBuilder
    {
        private readonly ExpressionConfiguration expressionConfiguration;

        public ParameterizedExpressionBuilder(ExpressionConfiguration expressionConfiguration)
        {
            this.expressionConfiguration = expressionConfiguration;
        }

        public IConfiguredSignatureExpressionBuilder HavingReturn(Type type, object defaultValue)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.expressionConfiguration.ReturnType = type;
            this.expressionConfiguration.ReturnDefaultValue = defaultValue;
            this.expressionConfiguration.ReturnLabelTarget
                = Expression.Label(type, $"{this.expressionConfiguration.ExpressionName}_ReturnLabel");

            return new ConfiguredSignatureExpressionBuilder(expressionConfiguration);
        }

        public IConfiguredSignatureExpressionBuilder HavingReturn<T>(object defaultValue)
            => this.HavingReturn(typeof(T), defaultValue);
    }
}