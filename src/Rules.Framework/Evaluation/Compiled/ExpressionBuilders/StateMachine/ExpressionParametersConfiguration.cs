namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionParametersConfiguration : IExpressionParametersConfiguration
    {
        public ExpressionParametersConfiguration()
        {
            this.Parameters = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);
        }

        public IDictionary<string, ParameterExpression> Parameters { get; }

        public ParameterExpression CreateParameter(string name, Type type)
        {
            if (this.Parameters.ContainsKey(name))
            {
                throw new InvalidOperationException($"A parameter for name '{name}' was already added.");
            }

            var parameterExpression = Expression.Parameter(type, name);
            this.Parameters.Add(name, parameterExpression);

            return parameterExpression;
        }

        public ParameterExpression CreateParameter<T>(string name)
            => this.CreateParameter(name, typeof(T));
    }
}