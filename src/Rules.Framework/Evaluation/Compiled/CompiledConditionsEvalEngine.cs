namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Generic;

    internal sealed class CompiledConditionsEvalEngine : IConditionsEvalEngine
    {
        private readonly IConditionsTreeAnalyzer conditionsTreeAnalyzer;
        private readonly RulesEngineOptions rulesEngineOptions;

        public CompiledConditionsEvalEngine(
            IConditionsTreeAnalyzer conditionsTreeAnalyzer,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public bool Eval(IConditionNode conditionNode, IDictionary<string, object> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            if (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey, out object conditionFuncAux))
            {
                throw new ArgumentException("Condition node does not contain compiled information.", nameof(conditionNode));
            }

            var conditionFunc = (Func<EvaluationContext, bool>)conditionFuncAux;
            var compiledConditionsEvaluationContext = new EvaluationContext(
                conditions,
                evaluationOptions.MatchMode,
                this.rulesEngineOptions.MissingConditionBehavior);
            return conditionFunc.Invoke(compiledConditionsEvaluationContext);
        }
    }
}