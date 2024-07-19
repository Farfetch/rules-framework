namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework;
    using Rules.Framework.Generic.ConditionNodes;

    internal sealed class GenericComposedConditionNodeValidator<TConditionType> : AbstractValidator<ComposedConditionNode<TConditionType>>
    {
        private readonly GenericValueConditionNodeValidator<TConditionType> valueConditionNodeValidator;

        public GenericComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TConditionType>();

            this.RuleForEach(c => c.ChildConditionNodes)
                .NotNull()
                .Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TConditionType, ComposedConditionNode<TConditionType>>
                {
                    ComposedConditionNodeValidator = this,
                    ValidationContext = cc,
                    ValueConditionNodeValidator = this.valueConditionNodeValidator
                }));
        }
    }
}