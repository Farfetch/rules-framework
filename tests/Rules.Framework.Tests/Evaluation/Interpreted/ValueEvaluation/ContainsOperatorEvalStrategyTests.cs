namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
    using System;
    using Xunit;

    public class ContainsOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            ContainsOperatorEvalStrategy sut = new ContainsOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Arrange
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndFox_ReturnsTrue()
        {
            // Arrange
            string expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            string expectedRightOperand = "fox";

            ContainsOperatorEvalStrategy sut = new ContainsOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndYellow_ReturnsFalse()
        {
            // Arrange
            string expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            string expectedRightOperand = "yellow";

            ContainsOperatorEvalStrategy sut = new ContainsOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }
    }
}