namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    internal sealed class ConditionsTreeCompiler<TConditionType> : IConditionsTreeCompiler<TConditionType>
    {
        private static readonly Type dictionaryOfConditionTypeAndObjectType = typeof(IDictionary<TConditionType, object>);
        private readonly IValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider;

        public ConditionsTreeCompiler(IValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider)
        {
            this.valueConditionNodeCompilerProvider = valueConditionNodeCompilerProvider;
        }

        public void Compile(IConditionNode<TConditionType> conditionNode)
        {
            ParameterExpression parameterExpression = Expression.Parameter(dictionaryOfConditionTypeAndObjectType, "conditions");
            this.CompileInternal(conditionNode, parameterExpression);
            conditionNode.Properties[ConditionNodeProperties.CompiledFlagKey] = true;
        }

        private void CompileInternal(
            IConditionNode<TConditionType> conditionNode,
            ParameterExpression parameterExpression)
        {
            switch (conditionNode)
            {
                case ComposedConditionNode<TConditionType> composedConditionNode:
                    foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        this.CompileInternal(childConditionNode, parameterExpression);
                    }
                    break;
                case ValueConditionNode<TConditionType> valueConditionNode:
                    this.CompileValueConditionNode(valueConditionNode, parameterExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }

        private void CompileValueConditionNode(
            ValueConditionNode<TConditionType> valueConditionNode,
            ParameterExpression parameterExpression)
        {
            var operatorMetadata = OperatorsMetadata.AllByOperator[valueConditionNode.Operator];

            foreach (string multiplicity in operatorMetadata.SupportedMultiplicities)
            {
                var valueConditionNodeCompiler = this.valueConditionNodeCompilerProvider.GetValueConditionNodeCompiler(multiplicity);

                var compiledCondition = valueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression);

                valueConditionNode.Properties[ConditionNodeProperties.GetCompiledDelegateKey(multiplicity)] = compiledCondition;
            }
        }
    }
}
