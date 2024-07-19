namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class NotContainsOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo stringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.NotContains}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.AndAlso(
                builder.NotEqual(args.LeftHandOperand, builder.Constant<object>(value: null)),
                builder.Not(builder.Call(
                    args.LeftHandOperand,
                    stringContainsMethodInfo,
                    new Expression[] { args.RightHandOperand })));
        }
    }
}