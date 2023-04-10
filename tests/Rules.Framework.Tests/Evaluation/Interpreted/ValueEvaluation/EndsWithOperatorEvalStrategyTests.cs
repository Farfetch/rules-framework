<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/EndsWithOperatorEvalStrategyTests.cs
namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/EndsWithOperatorEvalStrategyTests.cs
    using System;
    using Xunit;

    public class EndsWithOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            EndsWithOperatorEvalStrategy sut = new EndsWithOperatorEvalStrategy();

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
            string expectedRightOperand = "dog";

            EndsWithOperatorEvalStrategy sut = new EndsWithOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenStringsxpto12345emailpt_ReturnsFalse()
        {
            // Arrange
            string expectedLeftOperand = "xpto12345@email.pt";
            string expectedRightOperand = "@email.com";

            EndsWithOperatorEvalStrategy sut = new EndsWithOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenStringsxpto12345emailpt_ReturnsTrue()
        {
            // Arrange
            string expectedLeftOperand = "xpto12345@email.pt";
            string expectedRightOperand = "@email.pt";

            EndsWithOperatorEvalStrategy sut = new EndsWithOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }
    }
}