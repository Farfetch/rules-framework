namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    internal sealed class EqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            return Expression.Equal(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
