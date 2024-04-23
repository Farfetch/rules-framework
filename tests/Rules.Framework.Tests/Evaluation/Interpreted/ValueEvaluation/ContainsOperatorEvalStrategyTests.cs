namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class ContainsOperatorEvalStrategyTests
    {
        private readonly ContainsOperatorEvalStrategy operatorEvalStrategy;

        public ContainsOperatorEvalStrategyTests()
        {
            this.operatorEvalStrategy = new ContainsOperatorEvalStrategy();
        }

        public static IEnumerable<object[]> ArrayLeftOperandFailureCases => new[]
        {
            new object[] { new[] { true, }, false },
            new object[] { new[] { 6.5m, 7.1m, 8.6m, }, 8.1m },
            new object[] { new[] { 1, 2, 3, 4, 5, }, 10 },
            new object[] { new[] { "C", "F", "M", "Z", }, "A" },
        };

        public static IEnumerable<object[]> ArrayLeftOperandSuccessCases => new[]
        {
            new object[] { new[] { true, }, true },
            new object[] { new[] { 6.5m, 7.1m, 8.6m, }, 6.5m },
            new object[] { new[] { 1, 2, 3, 4, 5, }, 4 },
            new object[] { new[] { "C", "F", "M", "Z", }, "M" },
        };

        [Theory]
        [MemberData(nameof(ArrayLeftOperandFailureCases))]
        public void Eval_GivenArrayOfTypeAndType_ReturnsFalse(IEnumerable expectedLeftOperand, object expectedRightOperand)
        {
            // Act
            var actual = this.operatorEvalStrategy.Eval(expectedLeftOperand.Cast<object>(), expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ArrayLeftOperandSuccessCases))]
        public void Eval_GivenArrayOfTypeAndType_ReturnsTrue(IEnumerable expectedLeftOperand, object expectedRightOperand)
        {
            // Act
            var actual = this.operatorEvalStrategy.Eval(expectedLeftOperand.Cast<object>(), expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => this.operatorEvalStrategy.Eval(expectedLeftOperand, expectedRightOperand));

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

            // Act
            var actual = this.operatorEvalStrategy.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndYellow_ReturnsFalse()
        {
            // Arrange
            var expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            var expectedRightOperand = "yellow";

            // Act
            var actual = this.operatorEvalStrategy.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }
    }
}