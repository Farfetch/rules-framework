namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class ContainsManyToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Dictionary<Type, MethodInfo> containsLinqGenericMethodInfos = InitializeLinqContainsMethodInfos();
        private static readonly DataTypes[] supportedDataTypes = { DataTypes.Boolean, DataTypes.Decimal, DataTypes.Integer, DataTypes.String };

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!supportedDataTypes.Contains(args.DataTypeConfiguration.DataType))
            {
                throw new NotSupportedException(
                    $"The operator '{nameof(Operators.Contains)}' is not supported for data type '{args.DataTypeConfiguration.DataType}' on a many to one scenario.");
            }

            var containsMethodInfo = containsLinqGenericMethodInfos[args.DataTypeConfiguration.Type];

            return builder.AndAlso(
                builder.NotEqual(args.LeftHandOperand, builder.Constant<object>(value: null!)),
                builder.Call(
                    null!,
                    containsMethodInfo,
                    new Expression[] { args.LeftHandOperand, args.RightHandOperand }));
        }

        private static Dictionary<Type, MethodInfo> InitializeLinqContainsMethodInfos()
        {
            var genericMethodInfo = typeof(Enumerable)
                .GetMethods()
                .First(m => string.Equals(m.Name, nameof(Enumerable.Contains), StringComparison.Ordinal) && m.GetParameters().Length == 2);

            return new Dictionary<Type, MethodInfo>
            {
                { typeof(bool), genericMethodInfo.MakeGenericMethod(typeof(bool)) },
                { typeof(decimal), genericMethodInfo.MakeGenericMethod(typeof(decimal)) },
                { typeof(int), genericMethodInfo.MakeGenericMethod(typeof(int)) },
                { typeof(string), genericMethodInfo.MakeGenericMethod(typeof(string)) },
            };
        }
    }
}