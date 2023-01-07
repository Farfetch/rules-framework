namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal interface IConditionExpressionBuilder
    {
        Expression BuildConditionExpression(IImplementationExpressionBuilder builder, BuildConditionExpressionArgs args);
    }
}