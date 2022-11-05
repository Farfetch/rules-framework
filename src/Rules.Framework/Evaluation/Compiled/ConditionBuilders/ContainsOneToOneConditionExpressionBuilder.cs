namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal sealed class ContainsOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);
        private static readonly MethodInfo stringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (dataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.Contains}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            var returnLabelTarget = Expression.Label(booleanType);
            Expression notNullScenario = Expression.Call(
                leftHandOperandExpression,
                stringContainsMethodInfo,
                rightHandOperatorExpression);
            Expression ifNotNullExpression = Expression.IfThen(
                Expression.NotEqual(leftHandOperandExpression, Expression.Constant(value: null)),
                Expression.Return(returnLabelTarget, notNullScenario));

            return Expression.Block(new[] { ifNotNullExpression, Expression.Label(returnLabelTarget, Expression.Constant(value: false)) });
        }
    }
}
