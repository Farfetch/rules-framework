namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;

    internal interface INamedExpressionBuilder
    {
        IParameterizedExpressionBuilder WithoutParameters();

        IParameterizedExpressionBuilder WithParameters(Action<IExpressionParametersConfiguration> parametersConfigurationAction);
    }
}