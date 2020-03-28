namespace Rules.Framework.Builder.Validation
{
    using FluentValidation;
    using FluentValidation.Results;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal class RuleValidator<TContentType, TConditionType> : AbstractValidator<Rule<TContentType, TConditionType>>
    {
        private readonly ValueConditionNodeValidator<bool, TConditionType> booleanConditionNodeValidator;
        private readonly ComposedConditionNodeValidator<TConditionType> composedConditionNodeValidator;
        private readonly ValueConditionNodeValidator<decimal, TConditionType> decimalConditionNodeValidator;
        private readonly ValueConditionNodeValidator<int, TConditionType> integerConditionNodeValidator;
        private readonly ValueConditionNodeValidator<string, TConditionType> stringConditionNodeValidator;

        public RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator<TConditionType>();
            this.integerConditionNodeValidator = new ValueConditionNodeValidator<int, TConditionType>();
            this.decimalConditionNodeValidator = new ValueConditionNodeValidator<decimal, TConditionType>();
            this.stringConditionNodeValidator = new ValueConditionNodeValidator<string, TConditionType>();
            this.booleanConditionNodeValidator = new ValueConditionNodeValidator<bool, TConditionType>();

            this.RuleFor(r => r.ContentContainer).NotNull();
            this.RuleFor(r => r.DateBegin).NotEmpty();
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null);
            this.RuleFor(r => r.Name).NotNull().NotEmpty();
            this.RuleFor(r => r.Priority).GreaterThan(0);
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) =>
            {
                ValidationResult validationResult = null;

                switch (cn)
                {
                    case ComposedConditionNode<TConditionType> composedConditionNode:
                        validationResult = this.composedConditionNodeValidator.Validate(composedConditionNode);
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