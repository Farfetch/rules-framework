namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;

    internal sealed class RuleValidator : AbstractValidator<Rule>
    {
        private readonly ComposedConditionNodeValidator composedConditionNodeValidator;

        private readonly ValueConditionNodeValidator valueConditionNodeValidator;

        private RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator();
            this.valueConditionNodeValidator = new ValueConditionNodeValidator();

            this.RuleFor(r => r.ContentContainer).NotNull();
            this.RuleFor(r => r.DateBegin).NotEmpty();
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null);
            this.RuleFor(r => r.Name).NotNull().NotEmpty();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<Rule>
            {
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                ValidationContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator,
            }));
        }

        public static RuleValidator Instance { get; } = new RuleValidator();
    }
}