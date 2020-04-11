namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Core;

    internal class RuleValidator<TContentType, TConditionType> : AbstractValidator<Rule<TContentType, TConditionType>>
    {
        private readonly ValueConditionNodeValidator<bool, TConditionType> booleanConditionNodeValidator;
        private readonly ComposedConditionNodeValidator<TConditionType> composedConditionNodeValidator;
        private readonly ValueConditionNodeValidator<decimal, TConditionType> decimalConditionNodeValidator;
        private readonly ValueConditionNodeValidator<int, TConditionType> integerConditionNodeValidator;
        private readonly ValueConditionNodeValidator<string, TConditionType> stringConditionNodeValidator;

        public RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator<TConditionType>();
            this.integerConditionNodeValidator = new ValueConditionNodeValidator<int, TConditionType>();
            this.decimalConditionNodeValidator = new ValueConditionNodeValidator<decimal, TConditionType>();
            this.stringConditionNodeValidator = new ValueConditionNodeValidator<string, TConditionType>();
            this.booleanConditionNodeValidator = new ValueConditionNodeValidator<bool, TConditionType>();

            this.RuleFor(r => r.ContentContainer).NotNull();
            this.RuleFor(r => r.DateBegin).NotEmpty();
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null);
            this.RuleFor(r => r.Name).NotNull().NotEmpty();
            this.RuleFor(r => r.Priority).GreaterThan(0);
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<TConditionType>
            {
                BooleanConditionNodeValidator = this.booleanConditionNodeValidator,
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                CustomContext = cc,
                DecimalConditionNodeValidator = this.decimalConditionNodeValidator,
                IntegerConditionNodeValidator = this.integerConditionNodeValidator,
                StringConditionNodeValidator = this.stringConditionNodeValidator
            }));
        }
    }
}