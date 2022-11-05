namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class InOneToManyConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo enumerableGenericContains
            = typeof(Enumerable)
                .GetMethods()
                .FirstOrDefault(m => string.Equals(m.Name, "Contains", StringComparison.Ordinal) && m.GetParameters().Length == 2);

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            return Expression.Call(null, enumerableGenericContains.MakeGenericMethod(dataTypeConfiguration.Type), rightHandOperatorExpression, leftHandOperandExpression);
        }
    }
}
