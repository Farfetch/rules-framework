namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal interface IExpressionImplementationBuilder
    {
        IConfiguredExpressionBuilder SetImplementation(Action<IExpressionBlockBuilder> builder);
    }
}