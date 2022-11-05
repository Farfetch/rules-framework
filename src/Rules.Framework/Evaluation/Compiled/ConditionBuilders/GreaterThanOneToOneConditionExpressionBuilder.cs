namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Linq.Expressions;

    internal sealed class GreaterThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (!dataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.GreaterThan))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThan}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            return Expression.GreaterThan(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
