namespace Rules.Framework.Builder.Validation
{
    using FluentValidation.Results;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal static class ConditionNodeValidationExtensions
    {
        public static void PerformValidation<TConditionType>(this IConditionNode<TConditionType> conditionNode, ConditionNodeValidationArgs<TConditionType> conditionNodeValidationArgs)
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode<TConditionType> composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case IntegerConditionNode<TConditionType> integerConditionNode:
                    validationResult = conditionNodeValidationArgs.IntegerConditionNodeValidator.Validate(integerConditionNode);
                    break;

                case DecimalConditionNode<TConditionType> decimalConditionNode:
                    validationResult = conditionNodeValidationArgs.DecimalConditionNodeValidator.Validate(decimalConditionNode);
                    break;

                case StringConditionNode<TConditionType> stringConditionNode:
                    validationResult = conditionNodeValidationArgs.StringConditionNodeValidator.Validate(stringConditionNode);
                    break;

                case BooleanConditionNode<TConditionType> booleanConditionNode:
                    validationResult = conditionNodeValidationArgs.BooleanConditionNodeValidator.Validate(booleanConditionNode);
                    break;

                default:
                    return;
            }

            if (!validationResult.IsValid)
            {
                foreach (ValidationFailure validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.CustomContext.AddFailure(validationFailure);
                }
            }
        }
    }
}