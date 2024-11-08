namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;

    internal sealed class GenericConditionNodeValidationArgs<TConditionType, TValidationContext>
    {
        public GenericComposedConditionNodeValidator<TConditionType> ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public GenericValueConditionNodeValidator<TConditionType> ValueConditionNodeValidator { get; set; }
    }
}