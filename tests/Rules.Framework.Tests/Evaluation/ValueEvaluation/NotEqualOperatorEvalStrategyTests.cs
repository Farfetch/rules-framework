namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Evaluation.ValueEvaluation;

    [TestClass]
    public class NotEqualOperatorEvalStrategyTests
    {
        [TestMethod]
        public void NotEqualOperatorEvalStrategy_Eval_GivenAsIntegers1And1_ReturnsFalse()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            NotEqualOperatorEvalStrategy sut = new NotEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void NotEqualOperatorEvalStrategy_Eval_GivenAsIntegers1And2_ReturnsTrue()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            NotEqualOperatorEvalStrategy sut = new NotEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            Assert.IsTrue(actual);
        }
    }
}