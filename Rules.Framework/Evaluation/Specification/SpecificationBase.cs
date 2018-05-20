namespace Rules.Framework.Evaluation.Specification
{
    internal abstract class SpecificationBase<T> : ISpecification<T>
    {
        public ISpecification<T> And(ISpecification<T> otherSpecification)
        {
            return new AndSpecification<T>(this, otherSpecification);
        }

        public abstract bool IsSatisfiedBy(T input);

        public ISpecification<T> Or(ISpecification<T> otherSpecification)
        {
            return new OrSpecification<T>(this, otherSpecification);
        }
    }
}