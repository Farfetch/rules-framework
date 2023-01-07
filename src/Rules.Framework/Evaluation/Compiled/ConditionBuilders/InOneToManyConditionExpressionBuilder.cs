namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal sealed class InOneToManyConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo enumerableGenericContains
            = typeof(Enumerable)
                .GetMethods()
                .FirstOrDefault(m => string.Equals(m.Name, "Contains", StringComparison.Ordinal) && m.GetParameters().Length == 2);

        public Expression BuildConditionExpression(IImplementationExpressionBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.Call(
                instance: null,
                enumerableGenericContains.MakeGenericMethod(args.DataTypeConfiguration.Type),
                new Expression[] { args.RightHandOperand, args.LeftHandOperand });
        }
    }
}