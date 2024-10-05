namespace Rules.Framework.Builder.Validation
{
    using FluentValidation.Results;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;
    using Rules.Framework.Generic.ConditionNodes;

    internal static class ConditionNodeValidationExtensions
    {
        public static void PerformValidation<TCondition, TValidationContext>(this IConditionNode<TCondition> conditionNode, GenericConditionNodeValidationArgs<TCondition, TValidationContext> conditionNodeValidationArgs)
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode<TCondition> composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate((ValueConditionNode<TCondition>)conditionNode);
                    break;
            }

            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
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
                foreach (var validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.ValidationContext.AddFailure(validationFailure);
                }
            }
        }
    }
}