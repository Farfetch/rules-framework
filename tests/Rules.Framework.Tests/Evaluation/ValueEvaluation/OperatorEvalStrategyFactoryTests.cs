namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class OperatorEvalStrategyFactoryTests
    {
        [Theory(Skip = "Un-skip when operators using this eval strategy are created")]
        [InlineData(Operators.In, typeof(InOperatorEvalStrategy))]
        public void GetManyToManyOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IManyToManyOperatorEvalStrategy actual = sut.GetManyToManyOperatorEvalStrategy(expectedOperator);

            // Assert
            actual.Should().NotBeNull().And.BeOfType(expectedType);
        }

        [Fact]
        public void GetManyToManyOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetManyToManyOperatorEvalStrategy(expectedOperator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Operator evaluation is not supported for operator '-1' on the context of IManyToManyOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.In)]
        public void GetManyToManyOperatorEvalStrategy_GivenUnsupportedOperator_ThrowsNotSupportedException(Operators @operator)
        {
            // Arrange
            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetManyToManyOperatorEvalStrategy(@operator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Operator evaluation is not supported for operator '{@operator}' on the context of IManyToManyOperatorEvalStrategy.");
        }

        [Theory(Skip = "Un-skip when operators using this eval strategy are created")]
        [InlineData(Operators.In, typeof(InOperatorEvalStrategy))]
        public void GetManyToOneOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IManyToOneOperatorEvalStrategy actual = sut.GetManyToOneOperatorEvalStrategy(expectedOperator);

            // Assert
            actual.Should().NotBeNull().And.BeOfType(expectedType);
        }

        [Fact]
        public void GetManyToOneOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetManyToOneOperatorEvalStrategy(expectedOperator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Operator evaluation is not supported for operator '-1' on the context of IManyToOneOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.In)]
        public void GetManyToOneOperatorEvalStrategy_GivenUnsupportedOperator_ThrowsNotSupportedException(Operators @operator)
        {
            // Arrange
            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetManyToOneOperatorEvalStrategy(@operator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Operator evaluation is not supported for operator '{@operator}' on the context of IManyToOneOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.In, typeof(InOperatorEvalStrategy))]
        public void GetOneToManyOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IOneToManyOperatorEvalStrategy actual = sut.GetOneToManyOperatorEvalStrategy(expectedOperator);

            // Assert
            actual.Should().NotBeNull().And.BeOfType(expectedType);
        }

        [Fact]
        public void GetOneToManyOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetOneToManyOperatorEvalStrategy(expectedOperator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Operator evaluation is not supported for operator '-1' on the context of IOneToManyOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.Equal)]
        public void GetOneToManyOperatorEvalStrategy_GivenUnsupportedOperator_ThrowsNotSupportedException(Operators @operator)
        {
            // Arrange
            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetOneToManyOperatorEvalStrategy(@operator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Operator evaluation is not supported for operator '{@operator}' on the context of IOneToManyOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.Equal, typeof(EqualOperatorEvalStrategy))]
        [InlineData(Operators.NotEqual, typeof(NotEqualOperatorEvalStrategy))]
        [InlineData(Operators.GreaterThan, typeof(GreaterThanOperatorEvalStrategy))]
        [InlineData(Operators.GreaterThanOrEqual, typeof(GreaterThanOrEqualOperatorEvalStrategy))]
        [InlineData(Operators.LesserThan, typeof(LesserThanOperatorEvalStrategy))]
        [InlineData(Operators.LesserThanOrEqual, typeof(LesserThanOrEqualOperatorEvalStrategy))]
        [InlineData(Operators.Contains, typeof(ContainsOperatorEvalStrategy))]
        [InlineData(Operators.NotContains, typeof(NotContainsOperatorEvalStrategy))]
        [InlineData(Operators.StartsWith, typeof(StartsWithOperatorEvalStrategy))]
        [InlineData(Operators.EndsWith, typeof(EndsWithOperatorEvalStrategy))]
        [InlineData(Operators.CaseInsensitiveStartsWith, typeof(CaseInsensitiveStartsWithOperatorEvalStrategy))]
        [InlineData(Operators.CaseInsensitiveEndsWith, typeof(CaseInsensitiveEndsWithOperatorEvalStrategy))]
        [InlineData(Operators.NotStartsWith, typeof(NotStartsWithOperatorEvalStrategy))]
        [InlineData(Operators.NotEndsWith, typeof(NotEndsWithOperatorEvalStrategy))]
        public void GetOneToOneOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IOneToOneOperatorEvalStrategy actual = sut.GetOneToOneOperatorEvalStrategy(expectedOperator);

            // Assert
            actual.Should().NotBeNull().And.BeOfType(expectedType);
        }

        [Fact]
        public void GetOneToOneOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetOneToOneOperatorEvalStrategy(expectedOperator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Operator evaluation is not supported for operator '-1' on the context of IOneToOneOperatorEvalStrategy.");
        }

        [Theory]
        [InlineData(Operators.In)]
        public void GetOneToOneOperatorEvalStrategy_GivenUnsupportedOperator_ThrowsNotSupportedException(Operators @operator)
        {
            // Arrange
            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetOneToOneOperatorEvalStrategy(@operator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Operator evaluation is not supported for operator '{@operator}' on the context of IOneToOneOperatorEvalStrategy.");
        }
    }
}