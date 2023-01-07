namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal sealed class EqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IImplementationExpressionBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.Equal(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}