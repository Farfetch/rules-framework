namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    internal sealed class LesserThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            return Expression.LessThan(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
