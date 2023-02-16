namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Core;

    internal sealed class RuleValidator<TContentType, TConditionType> : AbstractValidator<Rule<TContentType, TConditionType>>
    {
        private static RuleValidator<TContentType, TConditionType> ruleValidator;

        private readonly ComposedConditionNodeValidator<TConditionType> composedConditionNodeValidator;

        private readonly ValueConditionNodeValidator<TConditionType> valueConditionNodeValidator;

        private RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator<TConditionType>();
            this.valueConditionNodeValidator = new ValueConditionNodeValidator<TConditionType>();

            this.RuleFor(r => r.ContentContainer).NotNull();
            this.RuleFor(r => r.DateBegin).NotEmpty();
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null);
            this.RuleFor(r => r.Name).NotNull().NotEmpty();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<TConditionType, Rule<TContentType, TConditionType>>
            {
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                ValidationContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator
            }));
        }

        public static RuleValidator<TContentType, TConditionType> Instance
        {
            get
            {
                ruleValidator ??= new RuleValidator<TContentType, TConditionType>();

                return ruleValidator;
            }
        }
    }
}