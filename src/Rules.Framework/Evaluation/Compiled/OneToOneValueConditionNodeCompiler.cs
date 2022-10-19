namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class OneToOneValueConditionNodeCompiler : ValueConditionNodeCompilerBase, IValueConditionNodeCompiler
    {
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public OneToOneValueConditionNodeCompiler(
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.conditionExpressionBuilderProvider = conditionExpressionBuilderProvider;
        }
        public Func<IDictionary<TConditionType, object>, bool> Compile<TConditionType>(
            ValueConditionNode<TConditionType> valueConditionNode,
            ParameterExpression parameterExpression)
        {
            DataTypeConfiguration dataTypeConfiguration = this.GetDataTypeConfiguration(valueConditionNode.DataType);

            var getConditionExpression = CreateGetConditionExpression(valueConditionNode.ConditionType, parameterExpression, dataTypeConfiguration);

            var leftOperandExpression = CreateConvertedObjectExpression(getConditionExpression, dataTypeConfiguration.Type, dataTypeConfiguration.Default);

            var rightOperandExpression = CreateConvertedObjectExpression(valueConditionNode.Operand, dataTypeConfiguration.Type, dataTypeConfiguration.Default);

            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(valueConditionNode.Operator, Multiplicities.OneToOne);
            var conditionExpression = conditionExpressionBuilder.BuildConditionExpression(leftOperandExpression, rightOperandExpression, dataTypeConfiguration);

            return Expression.Lambda<Func<IDictionary<TConditionType, object>, bool>>(conditionExpression, parameterExpression).Compile();
        }
    }
}
