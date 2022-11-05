namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Linq.Expressions;

    internal sealed class LesserThanOrEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (!dataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.LessThanOrEqual))
            {
                throw new NotSupportedException($"The operator '{Operators.LesserThanOrEqual}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            return Expression.LessThanOrEqual(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
