namespace Rules.Framework.Evaluation.Specification
{
    internal interface ISpecification<T>
    {
        ISpecification<T> And(ISpecification<T> otherSpecification);

        bool IsSatisfiedBy(T input);

        ISpecification<T> Or(ISpecification<T> otherSpecification);
    }
}