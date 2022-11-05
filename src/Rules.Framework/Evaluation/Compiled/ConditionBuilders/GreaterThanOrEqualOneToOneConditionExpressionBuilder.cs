namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Linq.Expressions;

    internal sealed class GreaterThanOrEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (!dataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.GreaterThanOrEqual))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThanOrEqual}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            return Expression.GreaterThanOrEqual(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
