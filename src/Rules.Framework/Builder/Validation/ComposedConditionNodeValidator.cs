namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;

    internal sealed class ComposedConditionNodeValidator : AbstractValidator<ComposedConditionNode>
    {
        private readonly ValueConditionNodeValidator valueConditionNodeValidator;

        public ComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new ValueConditionNodeValidator();

            this.RuleFor(c => c.LogicalOperator).IsContainedOn(LogicalOperators.And, LogicalOperators.Or);
            this.RuleForEach(c => c.ChildConditionNodes)
                .NotNull()
                .Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<ComposedConditionNode>
                {
                    ComposedConditionNodeValidator = this,
                    ValidationContext = cc,
                    ValueConditionNodeValidator = this.valueConditionNodeValidator
                }));
        }
    }
}