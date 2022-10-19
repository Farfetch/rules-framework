namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework.Core;

    internal class NotStartsWithOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);

        private static readonly MethodInfo startsWithMethodInfo = typeof(string)
            .GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) });

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (dataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.NotStartsWith}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            var returnLabelTarget = Expression.Label(booleanType);
            Expression notNullScenario = Expression.Not(Expression.Call(
                leftHandOperandExpression,
                startsWithMethodInfo,
                rightHandOperatorExpression,
                Expression.Constant(StringComparison.InvariantCulture)));
            Expression ifNotNullExpression = Expression.IfThen(
                Expression.NotEqual(leftHandOperandExpression, Expression.Constant(value: null)),
                Expression.Return(returnLabelTarget, notNullScenario));

            return Expression.Block(new[] { ifNotNullExpression, Expression.Label(returnLabelTarget, Expression.Constant(value: false)) });
        }
    }
}