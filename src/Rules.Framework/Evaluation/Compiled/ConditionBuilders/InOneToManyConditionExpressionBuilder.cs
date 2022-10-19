namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal class InOneToManyConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo enumerableGenericContains = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2);

        public Expression BuildConditionExpression(
            Expression leftHandOperandExpression,
            Expression rightHandOperatorExpression,
            DataTypeConfiguration dataTypeConfiguration)
        {
            return Expression.Call(null, enumerableGenericContains.MakeGenericMethod(dataTypeConfiguration.Type), rightHandOperatorExpression, leftHandOperandExpression);
        }
    }
}
