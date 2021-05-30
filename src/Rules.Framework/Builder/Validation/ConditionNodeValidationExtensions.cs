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

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate(conditionNode as ValueConditionNode<TConditionType>);
                    break;
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