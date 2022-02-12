namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;

    internal class ConditionNodeValidationArgs<TConditionType, TValidationContext>
    {
        public ComposedConditionNodeValidator<TConditionType> ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public ValueConditionNodeValidator<TConditionType> ValueConditionNodeValidator { get; set; }
    }
}