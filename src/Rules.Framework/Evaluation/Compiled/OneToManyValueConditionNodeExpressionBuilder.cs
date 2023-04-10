namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class OneToManyValueConditionNodeExpressionBuilder : IValueConditionNodeExpressionBuilder
    {
        private static readonly MethodInfo changeTypeMethodInfo = typeof(Convert)
            .GetMethod(
                nameof(Convert.ChangeType),
                new[] { typeof(object), typeof(Type), typeof(IFormatProvider) });

        private static readonly Type enumerableType = typeof(IEnumerable<>);
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public OneToManyValueConditionNodeExpressionBuilder(IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.conditionExpressionBuilderProvider = conditionExpressionBuilderProvider;
        }

        public void Build(
            IExpressionBlockBuilder builder,
            BuildValueConditionNodeExpressionArgs args)
        {
            var enumerableOfDataType = enumerableType.MakeGenericType(args.DataTypeConfiguration.Type);
            var coalescedLeftOperandExpression = builder.CreateVariable<object>("CoalescedLeftOperand");
            var convertedLeftOperandExpression = builder.CreateVariable("ConvertedLeftOperand", args.DataTypeConfiguration.Type);
            var convertedRightOperandExpression = builder.CreateVariable("ConvertedRightOperand", enumerableOfDataType);

            // Line 1.
            var fallbackExpression = builder.Constant<object>(value: null);
            builder.If(
                evaluation => evaluation.NotEqual(args.LeftOperandVariableExpression, fallbackExpression),
                then => then.Block(block => block.Assign(coalescedLeftOperandExpression, args.LeftOperandVariableExpression)),
                @else => @else.Block(block => block.Assign(coalescedLeftOperandExpression, block.Constant(args.DataTypeConfiguration.Default))));

            // line 2.
            var convertLeftOperandExpression = builder.ConvertChecked(
                builder.Call(
                    instance: null,
                    changeTypeMethodInfo,
                    new Expression[]
                    {
                        coalescedLeftOperandExpression,
                        builder.Constant(args.DataTypeConfiguration.Type),
                        builder.Constant<IFormatProvider>(CultureInfo.InvariantCulture),
                    }),
                args.DataTypeConfiguration.Type);
            builder.Assign(
                convertedLeftOperandExpression,
                convertLeftOperandExpression);

            // Line 3.
            builder.Assign(
                convertedRightOperandExpression,
                builder.ConvertChecked(args.RightOperandVariableExpression, enumerableOfDataType));

            // Line 4.
            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider
                .GetConditionExpressionBuilderFor(args.Operator, Multiplicities.OneToMany);
            var buildConditionExpressionArgs = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = args.DataTypeConfiguration,
                LeftHandOperand = convertedLeftOperandExpression,
                RightHandOperand = convertedRightOperandExpression,
            };
            builder.Assign(
                args.ResultVariableExpression,
                conditionExpressionBuilder.BuildConditionExpression(builder, buildConditionExpressionArgs));

            // Line 5.
            builder.AddExpression(builder.Empty());
        }
    }
}