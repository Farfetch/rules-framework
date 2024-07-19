namespace Rules.Framework.Builder.Validation
{
    using FluentValidation.Results;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;
    using Rules.Framework.Generic.ConditionNodes;

    internal static class ConditionNodeValidationExtensions
    {
        public static void PerformValidation<TConditionType, TValidationContext>(this IConditionNode<TConditionType> conditionNode, GenericConditionNodeValidationArgs<TConditionType, TValidationContext> conditionNodeValidationArgs)
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode<TConditionType> composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate((ValueConditionNode<TConditionType>)conditionNode);
                    break;
            }

            if (!validationResult.IsValid)
            {
                foreach (ValidationFailure validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.ValidationContext.AddFailure(validationFailure);
                }
            }
        }

        public static void PerformValidation<TValidationContext>(this IConditionNode conditionNode, ConditionNodeValidationArgs<TValidationContext> conditionNodeValidationArgs)
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate((ValueConditionNode)conditionNode);
                    break;
            }

            if (!validationResult.IsValid)
            {
                foreach (ValidationFailure validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.ValidationContext.AddFailure(validationFailure);
                }
            }
        }
    }
}