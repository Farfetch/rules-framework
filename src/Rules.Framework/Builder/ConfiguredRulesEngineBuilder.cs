namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
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
            var operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(this.rulesEngineOptions);
            var multiplicityEvaluator = new MultiplicityEvaluator();
            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<TConditionType>();

            var conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, multiplicityEvaluator, dataTypesConfigurationProvider);

            var deferredEval = new DeferredEval(conditionEvalDispatchProvider, this.rulesEngineOptions);

            var conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval, conditionsTreeAnalyzer);

            var conditionTypeExtractor = new ConditionTypeExtractor<TContentType, TConditionType>();

            ValidationProvider validationProvider = ValidationProvider.New()
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