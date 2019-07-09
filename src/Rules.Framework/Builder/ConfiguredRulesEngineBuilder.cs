namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;

    internal class ConfiguredRulesEngineBuilder<TContentType, TConditionType> : IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;
        private readonly RulesEngineOptions rulesEngineOptions;
        private readonly RulesEngineOptionsValidator rulesEngineOptionsValidator;

        public ConfiguredRulesEngineBuilder(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptions = RulesEngineOptions.NewWithDefaults;
            this.rulesEngineOptionsValidator = new RulesEngineOptionsValidator();
        }

        public RulesEngine<TContentType, TConditionType> Build()
        {
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
            IDeferredEval deferredEval = new DeferredEval(operatorEvalStrategyFactory, this.rulesEngineOptions);
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval);
            return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, this.rulesDataSource, this.rulesEngineOptions);
        }

        public IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            this.rulesEngineOptionsValidator.EnsureValid(this.rulesEngineOptions);

            return this;
        }
    }
}