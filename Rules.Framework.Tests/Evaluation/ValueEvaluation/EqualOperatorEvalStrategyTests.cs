using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rules.Framework.Evaluation.ValueEvaluation;

namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    [TestClass]
    public class EqualOperatorEvalStrategyTests
    {
        [TestMethod]
        public void EqualOperatorEvalStrategy_Eval_GivenAsIntegers1And1_ReturnsTrue()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            EqualOperatorEvalStrategy sut = new EqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void EqualOperatorEvalStrategy_Eval_GivenAsIntegers1And2_ReturnsFalse()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            EqualOperatorEvalStrategy sut = new EqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            Assert.IsFalse(actual);
        }
    }
}