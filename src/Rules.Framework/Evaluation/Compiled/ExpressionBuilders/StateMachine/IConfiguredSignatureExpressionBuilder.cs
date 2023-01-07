namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;

    internal interface IConfiguredSignatureExpressionBuilder
    {
        IConfiguredExpressionBuilder SetImplementation(Action<IImplementationExpressionBuilder> builder);
    }
}