namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class CaseInsensitiveStartsWithOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);

        private static readonly MethodInfo startsWithMethodInfo = typeof(string)
            .GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) });

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.CaseInsensitiveStartsWith}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.AndAlso(
                builder.NotEqual(args.LeftHandOperand, builder.Constant<object>(value: null)),
                builder.Call(
                    args.LeftHandOperand,
                    startsWithMethodInfo,
                    new Expression[] { args.RightHandOperand, builder.Constant(StringComparison.InvariantCultureIgnoreCase) }));
        }
    }
}