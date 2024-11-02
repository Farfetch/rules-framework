namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Generic;

    internal sealed class GenericRuleValidator<TRuleset, TCondition> : AbstractValidator<Rule<TRuleset, TCondition>>
    {
        private static GenericRuleValidator<TRuleset, TCondition> ruleValidator;

        private readonly GenericComposedConditionNodeValidator<TCondition> composedConditionNodeValidator;

        private readonly GenericValueConditionNodeValidator<TCondition> valueConditionNodeValidator;

        private GenericRuleValidator()
        {
            this.composedConditionNodeValidator = new GenericComposedConditionNodeValidator<TCondition>();
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TCondition>();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TCondition, Rule<TRuleset, TCondition>>
            {
                ComposedConditionNodeValidator = this.composedConditionNodeValidator,
                ValidationContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator,
            }));
        }

        public static GenericRuleValidator<TRuleset, TCondition> Instance
        {
            get
            {
                ruleValidator ??= new GenericRuleValidator<TRuleset, TCondition>();

                return ruleValidator;
            }
        }
    }
}