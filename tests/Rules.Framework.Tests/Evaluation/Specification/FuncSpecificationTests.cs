namespace Rules.Framework.Tests.Evaluation.Specification
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Rules.Framework.Evaluation.Specification;

    [TestClass]
    public class FuncSpecificationTests
    {
        [TestMethod]
        public void FuncSpecification_And_GivenOtherSpecification_ReturnsAndSpecification()
        {
            // Arrange
            Mock<ISpecification<object>> mockOtherSpecification = new Mock<ISpecification<object>>();
            ISpecification<object> expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.And(expectedOtherSpecification);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(AndSpecification<object>));
        }

        [TestMethod]
        public void FuncSpecification_IsSatisfiedBy_GivenFuncEvalsAsFalse_ReturnsFalse()
        {
            // Arrange
            object expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => false;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void FuncSpecification_IsSatisfiedBy_GivenFuncEvalsAsTrue_ReturnsTrue()
        {
            // Arrange
            object expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void FuncSpecification_Or_GivenOtherSpecification_ReturnsOrSpecification()
        {
            // Arrange
            Mock<ISpecification<object>> mockOtherSpecification = new Mock<ISpecification<object>>();
            ISpecification<object> expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            FuncSpecification<object> sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.Or(expectedOtherSpecification);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(OrSpecification<object>));
        }
    }
}