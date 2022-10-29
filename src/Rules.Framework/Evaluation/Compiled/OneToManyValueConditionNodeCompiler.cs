namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal sealed class OneToManyValueConditionNodeCompiler : ValueConditionNodeCompilerBase, IValueConditionNodeCompiler
    {
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public OneToManyValueConditionNodeCompiler(
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

            var rightOperandExpression = CreateConvertedArrayExpression(valueConditionNode.Operand, dataTypeConfiguration.Type);

            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(valueConditionNode.Operator, Multiplicities.OneToMany);
            var conditionExpression = conditionExpressionBuilder.BuildConditionExpression(leftOperandExpression, rightOperandExpression, dataTypeConfiguration);

            return Expression.Lambda<Func<IDictionary<TConditionType, object>, bool>>(conditionExpression, parameterExpression).Compile();
        }
    }
}
