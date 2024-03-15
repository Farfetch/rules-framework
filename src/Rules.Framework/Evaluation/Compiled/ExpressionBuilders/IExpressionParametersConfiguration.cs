namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal interface IExpressionParametersConfiguration
    {
        IReadOnlyDictionary<string, ParameterExpression> Parameters { get; }

        ParameterExpression CreateParameter<T>(string name);

        ParameterExpression CreateParameter(string name, Type type);
    }
}