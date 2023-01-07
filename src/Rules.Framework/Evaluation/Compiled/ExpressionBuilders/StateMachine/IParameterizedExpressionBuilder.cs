namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;

    internal interface IParameterizedExpressionBuilder
    {
        IConfiguredSignatureExpressionBuilder HavingReturn<T>(object defaultValue);

        IConfiguredSignatureExpressionBuilder HavingReturn(Type type, object defaultValue);
    }
}