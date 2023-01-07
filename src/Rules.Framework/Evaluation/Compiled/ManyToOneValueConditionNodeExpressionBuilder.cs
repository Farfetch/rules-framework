namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal sealed class ManyToOneValueConditionNodeExpressionBuilder : IValueConditionNodeExpressionBuilder
    {
        private static readonly MethodInfo changeTypeMethodInfo = typeof(Convert)
            .GetMethod(
                nameof(Convert.ChangeType),
                new[] { typeof(object), typeof(Type), typeof(IFormatProvider) });

        private static readonly Type enumerableType = typeof(IEnumerable<>);
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public ManyToOneValueConditionNodeExpressionBuilder(IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.conditionExpressionBuilderProvider = conditionExpressionBuilderProvider;
        }

        public void Build(
            IImplementationExpressionBuilder builder,
            BuildValueConditionNodeExpressionArgs args)
        {
            var enumerableOfDataType = enumerableType.MakeGenericType(args.DataTypeConfiguration.Type);
            var coalescedLeftOperandExpression = builder.CreateVariable<object>("coalescedLeftOperand");
            var convertedLeftOperandExpression = builder.CreateVariable("convertedLeftOperand", enumerableOfDataType);
            var convertedRightOperandExpression = builder.CreateVariable("convertedRightOperand", args.DataTypeConfiguration.Type);

            // Line 1.
            var fallbackExpression = builder.Constant<object>(value: null);
            builder.If(
                test => test.NotEqual(args.LeftOperandVariableExpression, fallbackExpression),
                then => then.Block(block => block.Assign(coalescedLeftOperandExpression, args.LeftOperandVariableExpression)),
                @else => @else.Block(block => block.Assign(coalescedLeftOperandExpression, block.Constant(args.DataTypeConfiguration.Default))));

            // line 2.
            builder.Assign(
                convertedLeftOperandExpression,
                builder.ConvertChecked(coalescedLeftOperandExpression, enumerableOfDataType));

            // Line 3.
            builder.Assign(
                convertedRightOperandExpression,
                builder.ConvertChecked(args.RightOperandVariableExpression, args.DataTypeConfiguration.Type));

            // Line 4.
            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider
                .GetConditionExpressionBuilderFor(args.Operator, Multiplicities.ManyToOne);
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