namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Generic.ConditionNodes;

    internal sealed class GenericValueConditionNodeValidator<TConditionType> : AbstractValidator<ValueConditionNode<TConditionType>>
    {
        public GenericValueConditionNodeValidator()
        {
            this.RuleFor(c => c.ConditionType)
                .NotEmpty()
                .IsInEnum()
                .When(c => c.ConditionType.GetType().IsEnum);
        }
    }
}