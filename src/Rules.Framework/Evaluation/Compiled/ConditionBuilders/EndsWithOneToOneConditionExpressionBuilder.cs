namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal sealed class EndsWithOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);

        private static readonly MethodInfo endsWithMethodInfo = typeof(string)
            .GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) });

        public Expression BuildConditionExpression(IImplementationExpressionBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.EndsWith}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.AndAlso(
                builder.NotEqual(args.LeftHandOperand, builder.Constant<object>(value: null)),
                builder.Call(
                    args.LeftHandOperand,
                    endsWithMethodInfo,
                    new Expression[] { args.RightHandOperand, builder.Constant(StringComparison.InvariantCulture) }));
        }
    }
}