namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using FluentValidation.Results;
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
            this.RuleForEach(c => c.ChildConditionNodes).Custom((cn, cc) =>
            {
                ValidationResult validationResult = null;

                switch (cn)
                {
                    case ComposedConditionNode<TConditionType> composedConditionNode:
                        validationResult = this.Validate(composedConditionNode);
                        break;

                    case IntegerConditionNode<TConditionType> integerConditionNode:
                        validationResult = this.integerConditionNodeValidator.Validate(integerConditionNode);
                        break;

                    case DecimalConditionNode<TConditionType> decimalConditionNode:
                        validationResult = this.decimalConditionNodeValidator.Validate(decimalConditionNode);
                        break;

                    case StringConditionNode<TConditionType> stringConditionNode:
                        validationResult = this.stringConditionNodeValidator.Validate(stringConditionNode);
                        break;

                    case BooleanConditionNode<TConditionType> booleanConditionNode:
                        validationResult = this.booleanConditionNodeValidator.Validate(booleanConditionNode);
                        break;

                    default:
                        return;
                }

                if (!validationResult.IsValid)
                {
                    foreach (ValidationFailure validationFailure in validationResult.Errors)
                    {
                        cc.AddFailure(validationFailure);
                    }
                }
            });
        }
    }
}