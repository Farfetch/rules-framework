namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal sealed class LesserThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IImplementationExpressionBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.LessThan))
            {
                throw new NotSupportedException($"The operator '{Operators.LesserThan}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.LessThan(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}