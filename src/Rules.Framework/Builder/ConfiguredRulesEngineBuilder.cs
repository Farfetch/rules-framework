namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
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
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();

            IDataTypesConfigurationProvider dataTypesConfigurationProvider = new DataTypesConfigurationProvider(this.rulesEngineOptions);
            IMultiplicityEvaluator multiplicityEvaluator = new MultiplicityEvaluator();
            IConditionsTreeAnalyzer<TConditionType> conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<TConditionType>();

            IConditionEvalDispatchProvider conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, multiplicityEvaluator, dataTypesConfigurationProvider);

            IDeferredEval deferredEval = new DeferredEval(conditionEvalDispatchProvider, this.rulesEngineOptions);

            IConditionsEvalEngine<TConditionType> conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval, conditionsTreeAnalyzer);

            IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor = new ConditionTypeExtractor<TContentType, TConditionType>();

            ValidationProvider validationProvider = ValidationProvider.New()
                .MapValidatorFor(new SearchArgsValidator<TContentType, TConditionType>());

            return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, this.rulesDataSource, validationProvider, this.rulesEngineOptions, conditionTypeExtractor);
        }

        public IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            RulesEngineOptionsValidator.Validate(this.rulesEngineOptions);

            return this;
        }
    }
}