namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;
    using System;
    using System.Linq.Expressions;

    internal sealed class LesserThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            if (!dataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.LessThan))
            {
                throw new NotSupportedException($"The operator '{Operators.LesserThan}' is not supported for data type '{dataTypeConfiguration.DataType}'.");
            }

            return Expression.LessThan(leftHandOperandExpression, rightHandOperatorExpression);
        }
    }
}
