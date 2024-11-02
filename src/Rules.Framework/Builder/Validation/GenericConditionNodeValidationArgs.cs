namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;

    internal sealed class GenericConditionNodeValidationArgs<TCondition, TValidationContext>
    {
        public GenericComposedConditionNodeValidator<TCondition> ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public GenericValueConditionNodeValidator<TCondition> ValueConditionNodeValidator { get; set; }
    }
}