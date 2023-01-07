namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Linq.Expressions;

    internal interface IExpressionParametersConfiguration
    {
        ParameterExpression CreateParameter<T>(string name);

        ParameterExpression CreateParameter(string name, Type type);
    }
}