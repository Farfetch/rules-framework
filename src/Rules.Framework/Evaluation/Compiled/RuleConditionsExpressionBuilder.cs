namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class RuleConditionsExpressionBuilder<TConditionType> : RuleConditionsExpressionBuilderBase, IRuleConditionsExpressionBuilder<TConditionType>
    {
        private static readonly MethodInfo conditionsGetterMethod = typeof(EvaluationContext<TConditionType>)
            .GetProperty("Conditions")
            .GetGetMethod();

        private static readonly MethodInfo evaluationContextMatchModeGetterMethod = typeof(EvaluationContext<TConditionType>).GetProperty("MatchMode").GetGetMethod();
        private static readonly MethodInfo evaluationContextMissingConditionsBehaviorGetterMethod = typeof(EvaluationContext<TConditionType>).GetProperty("MissingConditionBehavior").GetGetMethod();

        private static readonly MethodInfo getValueOrDefaultMethod = typeof(ConditionsValueLookupExtension)
            .GetMethod(nameof(ConditionsValueLookupExtension.GetValueOrDefault))
            .MakeGenericMethod(typeof(TConditionType));

        private readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;
        private readonly IValueConditionNodeExpressionBuilderProvider valueConditionNodeExpressionBuilderProvider;

        public RuleConditionsExpressionBuilder(
            IValueConditionNodeExpressionBuilderProvider valueConditionNodeExpressionBuilderProvider,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.valueConditionNodeExpressionBuilderProvider = valueConditionNodeExpressionBuilderProvider;
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
        }

        public Expression<Func<EvaluationContext<TConditionType>, bool>> BuildExpression(IConditionNode<TConditionType> rootConditionNode)
        {
            var expressionResult = ExpressionBuilder.NewExpression("EvaluateConditions")
                .WithParameters(p =>
                {
                    p.CreateParameter<EvaluationContext<TConditionType>>("evaluationContext");
                })
                .HavingReturn<bool>(defaultValue: false)
                .SetImplementation(x =>
                {
                    var resultVariableExpression = x.CreateVariable<bool>("Result");

                    this.BuildExpression(rootConditionNode, x);

                    x.Return(resultVariableExpression);
                })
                .Build();

            return Expression.Lambda<Func<EvaluationContext<TConditionType>, bool>>(
                body: expressionResult.Implementation,
                parameters: expressionResult.Parameters);
        }

        private static void BuildExpressionForBehaviorOnNullLeftOperand(IExpressionBlockBuilder builder)
        {
            var leftOperandVariableExpression = builder.GetVariable("LeftOperand");
            var resultVariableExpression = builder.GetVariable("Result");
            var jumpToLabelTarget = builder.GetLabelTarget("LabelEndValueConditionNode");
            var parameterExpression = builder.GetParameter("evaluationContext");

            builder.If(
                test => test.Equal(leftOperandVariableExpression, test.Constant<object>(value: null!)),
                then => then.Block(block1 =>
                {
                    block1.If(
                        test => test.Equal(test.Call(parameterExpression, evaluationContextMissingConditionsBehaviorGetterMethod), test.Constant(MissingConditionBehaviors.Discard)),
                        then => then.Block(block2 =>
                        {
                            block2.Assign(resultVariableExpression, block2.Constant(value: false));
                            block2.Goto(jumpToLabelTarget);
                        }));
                    block1.If(
                        test => test.Equal(test.Call(parameterExpression, evaluationContextMatchModeGetterMethod), test.Constant(MatchModes.Search)),
                        then => then.Block(block3 =>
                        {
                            block3.Assign(resultVariableExpression, then.Constant(value: true));
                            block3.Goto(jumpToLabelTarget);
                        }));
                }));
        }

        private void BuildExpression(IConditionNode<TConditionType> conditionNode, IExpressionBlockBuilder builder)
        {
            switch (conditionNode)
            {
                case ComposedConditionNode<TConditionType> composedConditionNode:
                    var conditionExpressions = new List<Expression>(composedConditionNode.ChildConditionNodes.Count());
                    var counter = 0;
                    foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        var scopeNameBuilder = new StringBuilder(builder.ScopeName);
                        _ = scopeNameBuilder.Length == 0 ? scopeNameBuilder.Append("cnd") : scopeNameBuilder.Append("InnerCnd");
                        var scopeName = scopeNameBuilder.Append(counter).ToString();
                        var blockExpression = builder.Block(scopeName, x =>
                        {
                            var childResultVariableExpression = x.CreateVariable<bool>("Result");
                            this.BuildExpression(childConditionNode, x);
                            conditionExpressions.Add(childResultVariableExpression);
                        });
                        builder.AddExpression(blockExpression);
                        counter++;
                    }

                    var conditionExpression = composedConditionNode.LogicalOperator switch
                    {
                        LogicalOperators.And => builder.AndAlso(conditionExpressions),
                        LogicalOperators.Or => builder.OrElse(conditionExpressions),
                        _ => throw new NotSupportedException($"Unsupported logical operator on composed condition node: '{conditionNode.LogicalOperator}'.")
                    };
                    var composedResultVariableExpression = builder.GetVariable("Result");
                    builder.Assign(composedResultVariableExpression, conditionExpression);
                    break;

                case ValueConditionNode<TConditionType> valueConditionNode:
                    // Variables, constants, and labels.
                    var leftOperandVariableExpression = builder.CreateVariable<object>("LeftOperand");
                    var rightOperandVariableExpression = builder.CreateVariable<object>("RightOperand");
                    var jumpToLabelTarget = builder.CreateLabelTarget("LabelEndValueConditionNode");
                    var parameterExpression = builder.GetParameter("evaluationContext");

                    // Line 1.
                    var getConditionsCallExpression = builder.Call(parameterExpression, conditionsGetterMethod);
                    var getConditionValueCallExpression = builder
                        .Call(instance: null!, getValueOrDefaultMethod, new Expression[] { getConditionsCallExpression, builder.Constant(valueConditionNode.ConditionType) });
                    builder.Assign(leftOperandVariableExpression, getConditionValueCallExpression);
                    // Line 2.
                    builder.Assign(rightOperandVariableExpression, builder.Constant(valueConditionNode.Operand));
                    // Line 3.
                    BuildExpressionForBehaviorOnNullLeftOperand(builder);
                    // Lines 4 and 5.
                    this.BuildFetchAndSwitchOverMultiplicity(builder, valueConditionNode);
                    // Line 6.
                    builder.Label(jumpToLabelTarget);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }

        private void BuildFetchAndSwitchOverMultiplicity(
            IExpressionBlockBuilder builder,
            ValueConditionNode<TConditionType> valueConditionNode)
        {
            var operatorConstantExpression = builder.Constant(valueConditionNode.Operator);
            var multiplicityVariableExpression = builder.CreateVariable("Multiplicity", typeof(string));
            var leftOperandVariableExpression = builder.GetVariable("LeftOperand");
            var rightOperandVariableExpression = builder.GetVariable("RightOperand");
            var resultVariableExpression = builder.GetVariable("Result");
            // Line 4.
            builder.Assign(multiplicityVariableExpression, builder.Call(
                instance: null!,
                multiplicityEvaluateMethod,
                new Expression[] { leftOperandVariableExpression, operatorConstantExpression, rightOperandVariableExpression }));
            // Line 5.
            builder.Switch(multiplicityVariableExpression, @switch =>
            {
                var dataTypeConfiguration = this.dataTypesConfigurationProvider.GetDataTypeConfiguration(valueConditionNode.DataType);
                var operatorMetadata = OperatorsMetadata.AllByOperator[valueConditionNode.Operator];

                foreach (var multiplicity in operatorMetadata.SupportedMultiplicities)
                {
                    string multiplicityTransformed = Regex.Replace(multiplicity, "\\b\\p{Ll}", match => match.Value.ToUpperInvariant(), RegexOptions.None, TimeSpan.FromSeconds(1)).Replace("-", string.Empty, StringComparison.Ordinal);
                    var scopeName = new StringBuilder(builder.ScopeName)
                        .Append(valueConditionNode.ConditionType)
                        .Append(multiplicityTransformed)
                        .ToString();
                    @switch.Case(
                        builder.Constant(multiplicity),
                        caseBuilder => caseBuilder.Block(scopeName, block =>
                        {
                            var valueConditionNodeExpressionBuilder = this.valueConditionNodeExpressionBuilderProvider
                                .GetExpressionBuilder(multiplicity);
                            var args = new BuildValueConditionNodeExpressionArgs
                            {
                                DataTypeConfiguration = dataTypeConfiguration,
                                LeftOperandVariableExpression = leftOperandVariableExpression,
                                Operator = operatorMetadata.Operator,
                                ResultVariableExpression = resultVariableExpression,
                                RightOperandVariableExpression = rightOperandVariableExpression,
                            };
                            valueConditionNodeExpressionBuilder.Build(
                                block,
                                args);
                        }));
                }
                @switch.Default(defaultBuilder => defaultBuilder.Empty());
            });
        }
    }
}