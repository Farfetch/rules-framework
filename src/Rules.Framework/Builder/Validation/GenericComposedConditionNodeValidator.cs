namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Generic.ConditionNodes;

    internal sealed class GenericComposedConditionNodeValidator<TCondition> : AbstractValidator<ComposedConditionNode<TCondition>>
    {
        private readonly GenericValueConditionNodeValidator<TCondition> valueConditionNodeValidator;

        public GenericComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TCondition>();

            this.RuleForEach(c => c.ChildConditionNodes)
                .NotNull()
                .Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TCondition, ComposedConditionNode<TCondition>>
                {
                    ComposedConditionNodeValidator = this,
                    ValidationContext = cc,
                    ValueConditionNodeValidator = this.valueConditionNodeValidator,
                }));
        }
    }
}