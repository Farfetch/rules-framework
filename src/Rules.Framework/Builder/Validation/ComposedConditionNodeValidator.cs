namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal class ComposedConditionNodeValidator<TConditionType> : AbstractValidator<ComposedConditionNode<TConditionType>>
    {
        private readonly ValueConditionNodeValidator<bool, TConditionType> booleanConditionNodeValidator;
        private readonly ValueConditionNodeValidator<decimal, TConditionType> decimalConditionNodeValidator;
        private readonly ValueConditionNodeValidator<int, TConditionType> integerConditionNodeValidator;
        private readonly ValueConditionNodeValidator<string, TConditionType> stringConditionNodeValidator;

        public ComposedConditionNodeValidator()
        {
            this.integerConditionNodeValidator = new ValueConditionNodeValidator<int, TConditionType>();
            this.decimalConditionNodeValidator = new ValueConditionNodeValidator<decimal, TConditionType>();
            this.stringConditionNodeValidator = new ValueConditionNodeValidator<string, TConditionType>();
            this.booleanConditionNodeValidator = new ValueConditionNodeValidator<bool, TConditionType>();

            this.RuleFor(c => c.LogicalOperator).IsContainedOn(LogicalOperators.And, LogicalOperators.Or);
            this.RuleForEach(c => c.ChildConditionNodes).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<TConditionType>
            {
                BooleanConditionNodeValidator = this.booleanConditionNodeValidator,
                ComposedConditionNodeValidator = this,
                CustomContext = cc,
                DecimalConditionNodeValidator = this.decimalConditionNodeValidator,
                IntegerConditionNodeValidator = this.integerConditionNodeValidator,
                StringConditionNodeValidator = this.stringConditionNodeValidator
            }));
        }
    }
}