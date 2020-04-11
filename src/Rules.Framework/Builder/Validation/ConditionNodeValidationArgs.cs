namespace Rules.Framework.Builder.Validation
{
    using FluentValidation.Validators;

    internal class ConditionNodeValidationArgs<TConditionType>
    {
        public ValueConditionNodeValidator<bool, TConditionType> BooleanConditionNodeValidator { get; set; }
        public ComposedConditionNodeValidator<TConditionType> ComposedConditionNodeValidator { get; set; }
        public CustomContext CustomContext { get; set; }
        public ValueConditionNodeValidator<decimal, TConditionType> DecimalConditionNodeValidator { get; set; }
        public ValueConditionNodeValidator<int, TConditionType> IntegerConditionNodeValidator { get; set; }
        public ValueConditionNodeValidator<string, TConditionType> StringConditionNodeValidator { get; set; }
    }
}