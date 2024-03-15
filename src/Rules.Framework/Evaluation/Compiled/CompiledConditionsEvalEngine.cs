namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal sealed class CompiledConditionsEvalEngine<TConditionType> : IConditionsEvalEngine<TConditionType>
    {
        private readonly IConditionsTreeAnalyzer<TConditionType> conditionsTreeAnalyzer;
        private readonly RulesEngineOptions rulesEngineOptions;

        public CompiledConditionsEvalEngine(
            IConditionsTreeAnalyzer<TConditionType> conditionsTreeAnalyzer,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public bool Eval(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            if (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey, out object conditionFuncAux))
            {
                throw new ArgumentException("Condition node does not contain compiled information.", nameof(conditionNode));
            }

            var conditionFunc = (Func<EvaluationContext<TConditionType>, bool>)conditionFuncAux;
            var compiledConditionsEvaluationContext = new EvaluationContext<TConditionType>(
                conditions,
                evaluationOptions.MatchMode,
                this.rulesEngineOptions.MissingConditionBehavior);
            return conditionFunc.Invoke(compiledConditionsEvaluationContext);
        }
    }
}