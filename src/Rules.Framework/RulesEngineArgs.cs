namespace Rules.Framework
{
    using Rules.Framework.Evaluation;
    using Rules.Framework.Source;
    using Rules.Framework.Validation;

    internal class RulesEngineArgs<TContentType, TConditionType>
    {
        public IConditionsEvalEngine<TConditionType> ConditionsEvalEngine { get; set; }

        public IConditionTypeExtractor<TContentType, TConditionType> ConditionTypeExtractor { get; set; }

        public RulesEngineOptions Options { get; set; }

        public IRulesSource<TContentType, TConditionType> RulesSource { get; set; }

        public IValidatorProvider ValidatorProvider { get; set; }
    }
}