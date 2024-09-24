namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;

    internal sealed class ConditionNodeValidationArgs<TValidationContext>
    {
        public ComposedConditionNodeValidator ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public ValueConditionNodeValidator ValueConditionNodeValidator { get; set; }
    }
}