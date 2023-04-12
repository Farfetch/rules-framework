namespace Rules.Framework.Tests.Evaluation.Interpreted
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Interpreted;
    using Xunit;

    public class FuncSpecificationTests
    {
        [Fact]
        public void And_GivenOtherSpecification_ReturnsAndSpecification()
        {
            // Arrange
            Mock<ISpecification<object>> mockOtherSpecification = new Mock<ISpecification<object>>();
            ISpecification<object> expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.And(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<AndSpecification<object>>();
        }

        [Fact]
        public void IsSatisfiedBy_GivenFuncEvalsAsFalse_ReturnsFalse()
        {
            // Arrange
            object expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => false;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSatisfiedBy_GivenFuncEvalsAsTrue_ReturnsTrue()
        {
            // Arrange
            object expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void Or_GivenOtherSpecification_ReturnsOrSpecification()
        {
            // Arrange
            Mock<ISpecification<object>> mockOtherSpecification = new Mock<ISpecification<object>>();
            ISpecification<object> expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.Or(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<OrSpecification<object>>();
        }
    }
}