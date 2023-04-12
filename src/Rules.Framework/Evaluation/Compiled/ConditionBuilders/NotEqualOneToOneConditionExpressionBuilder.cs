namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class NotEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.NotEqual(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}