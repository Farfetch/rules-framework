namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class NotContainsOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            var sut = new NotContainsOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Arrange
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndFox_ReturnsTrue()
        {
            // Arrange
            var expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            var expectedRightOperand = "fox";

            var sut = new NotContainsOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndYellow_ReturnsFalse()
        {
            // Arrange
            var expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            var expectedRightOperand = "yellow";

            var sut = new NotContainsOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }
    }
}