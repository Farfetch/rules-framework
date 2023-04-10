<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/GreaterThanOrEqualOperatorEvalStrategyTests.cs
namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/GreaterThanOrEqualOperatorEvalStrategyTests.cs
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/GreaterThanOrEqualOperatorEvalStrategyTests.cs
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/GreaterThanOrEqualOperatorEvalStrategyTests.cs
    using Xunit;

    public class GreaterThanOrEqualOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers0And1_ReturnsFalse()
        {
            // Assert
            int expectedLeftOperand = 0;
            int expectedRightOperand = 1;

            GreaterThanOrEqualOperatorEvalStrategy sut = new GreaterThanOrEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsTrue()
        {
            // Assert
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            GreaterThanOrEqualOperatorEvalStrategy sut = new GreaterThanOrEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenAsIntegers2And1_ReturnsTrue()
        {
            // Assert
            int expectedLeftOperand = 2;
            int expectedRightOperand = 1;

            GreaterThanOrEqualOperatorEvalStrategy sut = new GreaterThanOrEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [MemberData(nameof(UnsupportedOperandTypesCombinations))]
        [Theory]
        public void Eval_GivenUnsupportedOperandTypes_ThrowsNotSupportedException(object leftOperand, object rightOperand)
        {
            // Arrange
            GreaterThanOrEqualOperatorEvalStrategy sut = new GreaterThanOrEqualOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}