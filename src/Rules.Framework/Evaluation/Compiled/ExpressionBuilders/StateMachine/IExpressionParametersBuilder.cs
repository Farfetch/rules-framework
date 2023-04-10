namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal interface IExpressionParametersBuilder
    {
        IExpressionReturnBuilder WithoutParameters();

        IExpressionReturnBuilder WithParameters(Action<IExpressionParametersConfiguration> parametersConfigurationAction);
    }
}