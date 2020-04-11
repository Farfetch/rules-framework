namespace Rules.Framework.Tests.Evaluation.Specification
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Specification;
    using Xunit;

    public class AndSpecificationTests
    {
        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsFalse_ShortCircuitsDoesNotEvalRightSpecificationAndReturnsFalse()
        {
            // Arrange
            object expectedInput = new object();

            Mock<ISpecification<object>> mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);
            Mock<ISpecification<object>> mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);

            ISpecification<object> expectedLeftSpecification = mockLeftSpecification.Object;
            ISpecification<object> expectedRightSpecification = mockRightSpecification.Object;

            AndSpecification<object> sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Never());
        }

        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsTrueAndRightSpecificationEvalAsFalse_EvalsBothSpecificationsAndReturnsFalse()
        {
            // Arrange
            object expectedInput = new object();

            Mock<ISpecification<object>> mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);
            Mock<ISpecification<object>> mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);

            ISpecification<object> expectedLeftSpecification = mockLeftSpecification.Object;
            ISpecification<object> expectedRightSpecification = mockRightSpecification.Object;

            AndSpecification<object> sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }

        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsTrueAndRightSpecificationEvalAsTrue_EvalsBothSpecificationsAndReturnsTrue()
        {
            // Arrange
            object expectedInput = new object();

            Mock<ISpecification<object>> mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);
            Mock<ISpecification<object>> mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);

            ISpecification<object> expectedLeftSpecification = mockLeftSpecification.Object;
            ISpecification<object> expectedRightSpecification = mockRightSpecification.Object;

            AndSpecification<object> sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }
    }
}