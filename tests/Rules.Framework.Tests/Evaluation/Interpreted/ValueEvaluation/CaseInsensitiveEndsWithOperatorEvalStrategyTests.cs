<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/CaseInsensitiveEndsWithOperatorEvalStrategyTests.cs
namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/CaseInsensitiveEndsWithOperatorEvalStrategyTests.cs
    using Xunit;

    public class CaseInsensitiveEndsWithOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            CaseInsensitiveEndsWithOperatorEvalStrategy sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Theory]
        [InlineData("Random Message One Two Three", "Threee")]
        [InlineData("Random Message One Two Three", "Random")]
        [InlineData("Random Message One Two Three", "The")]
        public void Eval_GivenStringsRandomMessageOneTwoThree_ReturnsFalse(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            CaseInsensitiveEndsWithOperatorEvalStrategy sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog", "dog")]
        [InlineData("The quick brown fox jumps over the lazy Dog", "dog")]
        [InlineData("The quick brown fox jumps over the lazy dog", "Dog")]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndFox_CaseInsensitive_ReturnsTrue(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            CaseInsensitiveEndsWithOperatorEvalStrategy sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeTrue();
        }
    }
}