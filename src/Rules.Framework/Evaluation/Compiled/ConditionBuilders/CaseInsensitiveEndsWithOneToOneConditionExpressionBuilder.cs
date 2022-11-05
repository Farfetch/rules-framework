namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);
        private static readonly MethodInfo endsWithMethodInfo = typeof(string)
            .GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) });

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (dataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.CaseInsensitiveEndsWith}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            var returnLabelTarget = Expression.Label(booleanType);
            Expression notNullScenario = Expression.Call(
                leftHandOperandExpression,
                endsWithMethodInfo,
                rightHandOperatorExpression,
                Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
            Expression ifNotNullExpression = Expression.IfThen(
                Expression.NotEqual(leftHandOperandExpression, Expression.Constant(value: null)),
                Expression.Return(returnLabelTarget, notNullScenario));

            return Expression.Block(new[] { ifNotNullExpression, Expression.Label(returnLabelTarget, Expression.Constant(value: false)) });
        }
    }
}
