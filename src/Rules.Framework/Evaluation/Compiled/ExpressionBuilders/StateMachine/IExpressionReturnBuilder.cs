namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;

    internal interface IExpressionReturnBuilder
    {
        IExpressionImplementationBuilder HavingReturn<T>(object defaultValue);

        IExpressionImplementationBuilder HavingReturn(Type type, object defaultValue);
    }
}