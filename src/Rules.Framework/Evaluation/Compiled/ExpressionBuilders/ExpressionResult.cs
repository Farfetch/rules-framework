namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionResult
    {
        public ExpressionResult(
            string expressionName,
            Expression implementation,
            IEnumerable<ParameterExpression> parameters,
            Type returnType)
        {
            this.ExpressionName = expressionName;
            this.Implementation = implementation;
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

        public string ExpressionName { get; }

        public Expression Implementation { get; }

        public IEnumerable<ParameterExpression> Parameters { get; }

        public Type ReturnType { get; }
    }
}