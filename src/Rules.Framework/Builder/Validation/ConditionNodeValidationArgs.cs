namespace Rules.Framework.Builder.Validation
{
    using FluentValidation.Validators;

    internal class ConditionNodeValidationArgs<TConditionType>
    {
        public ComposedConditionNodeValidator<TConditionType> ComposedConditionNodeValidator { get; set; }
        public CustomContext CustomContext { get; set; }
        public ValueConditionNodeValidator<TConditionType> ValueConditionNodeValidator { get; set; }
    }
}