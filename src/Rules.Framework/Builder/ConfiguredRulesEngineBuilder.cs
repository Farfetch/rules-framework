namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;

    internal class ConfiguredRulesEngineBuilder<TContentType, TConditionType> : IConfiguredRulesEngineBuilder<TContentType, TConditionType>
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
            IDeferredEval deferredEval = new DeferredEval(operatorEvalStrategyFactory, this.rulesEngineOptions);
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval);
            return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, this.rulesDataSource, this.rulesEngineOptions);
        }

        public IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            RulesEngineOptionsValidator.EnsureValid(this.rulesEngineOptions);

            return this;
        }
    }
}