namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal interface IConditionExpressionBuilder
    {
        Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args);
    }
}