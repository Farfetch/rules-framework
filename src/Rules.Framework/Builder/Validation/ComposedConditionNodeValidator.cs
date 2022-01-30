namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal class ComposedConditionNodeValidator<TConditionType> : AbstractValidator<ComposedConditionNode<TConditionType>>
    {
        private readonly ValueConditionNodeValidator<TConditionType> valueConditionNodeValidator;

        public ComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new ValueConditionNodeValidator<TConditionType>();

            this.RuleFor(c => c.LogicalOperator).IsContainedOn(LogicalOperators.And, LogicalOperators.Or);
            this.RuleForEach(c => c.ChildConditionNodes).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<TConditionType, ComposedConditionNode<TConditionType>>
            {
                ComposedConditionNodeValidator = this,
                ValidationContext = cc,
                ValueConditionNodeValidator = this.valueConditionNodeValidator
            }));
        }
    }
}