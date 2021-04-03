namespace Rules.Framework.Validation
{
    using FluentValidation;

    internal interface IValidatorProvider
    {
        IValidator<T> GetValidatorFor<T>();
    }
}