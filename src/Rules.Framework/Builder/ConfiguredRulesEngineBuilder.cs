namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Classic;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Source;
    using Rules.Framework.Validation;

    internal sealed class ConfiguredRulesEngineBuilder<TContentType, TConditionType> : IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;
        private readonly RulesEngineOptions rulesEngineOptions;

        public ConfiguredRulesEngineBuilder(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
        }

        public RulesEngine<TContentType, TConditionType> Build()
        {
            var rulesSourceMiddlewares = new List<IRulesSourceMiddleware<TContentType, TConditionType>>();
            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(this.rulesEngineOptions);
            var multiplicityEvaluator = new MultiplicityEvaluator();

            IConditionsEvalEngine<TConditionType> conditionsEvalEngine;

            if (this.rulesEngineOptions.EnableCompilation)
            {
                var conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();
                var valueConditionNodeCompilerProvider = new ValueConditionNodeCompilerProvider(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);
                var conditionsTreeCompiler = new ConditionsTreeCompiler<TConditionType>(valueConditionNodeCompilerProvider);
                conditionsEvalEngine = new CompiledConditionsEvalEngine<TConditionType>(conditionsTreeCompiler, multiplicityEvaluator, this.rulesEngineOptions);
            }
            else
            {
                var operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
                var conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, multiplicityEvaluator, dataTypesConfigurationProvider);
                var deferredEval = new DeferredEval(conditionEvalDispatchProvider, this.rulesEngineOptions);
                var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<TConditionType>();
                conditionsEvalEngine = new ClassicConditionsEvalEngine<TConditionType>(deferredEval, conditionsTreeAnalyzer);
            }

            var conditionTypeExtractor = new ConditionTypeExtractor<TContentType, TConditionType>();

            var validationProvider = ValidationProvider.New()
                .MapValidatorFor(new SearchArgsValidator<TContentType, TConditionType>());

            var orderedMiddlewares = rulesSourceMiddlewares
                .Reverse<IRulesSourceMiddleware<TContentType, TConditionType>>();
            var rulesSource = new RulesSource<TContentType, TConditionType>(this.rulesDataSource, orderedMiddlewares);

            return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, rulesSource, validationProvider, this.rulesEngineOptions, conditionTypeExtractor);
        }

        public IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            RulesEngineOptionsValidator.Validate(this.rulesEngineOptions);

            return this;
        }
    }
}