namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Generic;

    internal sealed class GenericRuleValidator<TContentType, TConditionType> : AbstractValidator<Rule<TContentType, TConditionType>>
    {
        private static GenericRuleValidator<TContentType, TConditionType> ruleValidator;

        private readonly GenericComposedConditionNodeValidator<TConditionType> composedConditionNodeValidator;

        private readonly GenericValueConditionNodeValidator<TConditionType> valueConditionNodeValidator;

        private GenericRuleValidator()
        {
            this.composedConditionNodeValidator = new GenericComposedConditionNodeValidator<TConditionType>();
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TConditionType>();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TConditionType, Rule<TContentType, TConditionType>>
            {
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                ValidationContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator,
            }));
        }

        public static GenericRuleValidator<TContentType, TConditionType> Instance
        {
            get
            {
                ruleValidator ??= new GenericRuleValidator<TContentType, TConditionType>();

                return ruleValidator;
            }
        }
    }
}