using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rules.Framework.Evaluation.Specification;

namespace Rules.Framework.Tests.Evaluation.Specification
{
    [TestClass]
    public class OrSpecificationTests
    {
        [TestMethod]
        public void OrSpecification_IsSatisfiedBy_GivenLeftSpecificationEvalAsFalse_EvalsRightSpecificationAndReturnsFalse()
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

            OrSpecification<object> sut = new OrSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            Assert.IsFalse(actual);

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }

        [TestMethod]
        public void OrSpecification_IsSatisfiedBy_GivenLeftSpecificationEvalAsFalseAndRightSpecificationEvalAsTrue_EvalsBothSpecificationsAndReturnsTrue()
        {
            // Arrange
            object expectedInput = new object();

            Mock<ISpecification<object>> mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);
            Mock<ISpecification<object>> mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);

            ISpecification<object> expectedLeftSpecification = mockLeftSpecification.Object;
            ISpecification<object> expectedRightSpecification = mockRightSpecification.Object;

            OrSpecification<object> sut = new OrSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            Assert.IsTrue(actual);

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }

        [TestMethod]
        public void OrSpecification_IsSatisfiedBy_GivenLeftSpecificationEvalAsTrueAndRightSpecificationEvalAsFalse_ShortCircuitsDoesNotEvalRightSpecificationAndReturnsFalse()
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

            OrSpecification<object> sut = new OrSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            bool actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            Assert.IsTrue(actual);

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Never());
        }
    }
}