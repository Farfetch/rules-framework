namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal sealed class NotContainsOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo stringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            return Expression.Not(Expression.Call(leftHandOperandExpression, stringContainsMethodInfo, rightHandOperatorExpression));
        }
    }
}
