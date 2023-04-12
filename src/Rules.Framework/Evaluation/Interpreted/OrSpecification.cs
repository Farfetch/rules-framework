namespace Rules.Framework.Evaluation.Interpreted
{
    internal sealed class OrSpecification<T> : SpecificationBase<T>
    {
        private readonly ISpecification<T> leftSpecification;

        private readonly ISpecification<T> rightSpecification;

        public OrSpecification(ISpecification<T> leftSpecification, ISpecification<T> rightSpecification)
        {
            this.leftSpecification = leftSpecification;
            this.rightSpecification = rightSpecification;
        }

        public override bool IsSatisfiedBy(T input)
        {
            return this.leftSpecification.IsSatisfiedBy(input) || this.rightSpecification.IsSatisfiedBy(input);
        }
    }
}