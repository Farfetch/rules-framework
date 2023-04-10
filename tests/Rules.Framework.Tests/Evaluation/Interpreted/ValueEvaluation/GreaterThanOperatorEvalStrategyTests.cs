<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/GreaterThanOperatorEvalStrategyTests.cs
namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
========
namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/GreaterThanOperatorEvalStrategyTests.cs
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/GreaterThanOperatorEvalStrategyTests.cs
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/GreaterThanOperatorEvalStrategyTests.cs
    using Xunit;

    public class GreaterThanOperatorEvalStrategyTests
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

            GreaterThanOperatorEvalStrategy sut = new GreaterThanOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsFalse()
        {
            // Assert
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            GreaterThanOperatorEvalStrategy sut = new GreaterThanOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers2And1_ReturnsTrue()
        {
            // Assert
            int expectedLeftOperand = 2;
            int expectedRightOperand = 1;

            GreaterThanOperatorEvalStrategy sut = new GreaterThanOperatorEvalStrategy();

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
            GreaterThanOperatorEvalStrategy sut = new GreaterThanOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}