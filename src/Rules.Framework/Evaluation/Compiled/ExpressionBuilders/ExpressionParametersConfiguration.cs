namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionParametersConfiguration : IExpressionParametersConfiguration
    {
        private readonly Dictionary<string, ParameterExpression> parameters;

        public ExpressionParametersConfiguration()
        {
            this.parameters = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);
        }

        public IReadOnlyDictionary<string, ParameterExpression> Parameters => parameters;

        public ParameterExpression CreateParameter(string name, Type type)
        {
            if (this.parameters.ContainsKey(name))
            {
                throw new InvalidOperationException($"A parameter for name '{name}' was already added.");
            }

            var parameterExpression = Expression.Parameter(type, name);
            this.parameters.Add(name, parameterExpression);

            return parameterExpression;
        }

        public ParameterExpression CreateParameter<T>(string name)
            => this.CreateParameter(name, typeof(T));
    }
}