namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Core;

    internal class RuleValidator<TContentType, TConditionType> : AbstractValidator<Rule<TContentType, TConditionType>>
    {
        private readonly ComposedConditionNodeValidator<TConditionType> composedConditionNodeValidator;
        private readonly ValueConditionNodeValidator<TConditionType> valueConditionNodeValidator;

        public RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator<TConditionType>();
            this.valueConditionNodeValidator = new ValueConditionNodeValidator<TConditionType>();

            this.RuleFor(r => r.ContentContainer).NotNull();
            this.RuleFor(r => r.DateBegin).NotEmpty();
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null);
            this.RuleFor(r => r.Name).NotNull().NotEmpty();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<TConditionType>
            {
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                CustomContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator
            }));
        }
    }
}