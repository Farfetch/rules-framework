namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System.Linq.Expressions;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionResultTests
    {
        [Fact]
        public void Ctor_GivenParameters_ReturnsNewInstanceWithExactValuesProvided()
        {
            // Arrange
            var expectedExpressionName = "TestExpression";
            var expectedImplementation = Expression.Constant(true);
            var expectedParameters = new[] { Expression.Parameter(typeof(string)), Expression.Parameter(typeof(int)) };
            var expectedReturnType = typeof(bool);

            // Act
            var actual = new ExpressionResult(expectedExpressionName, expectedImplementation, expectedParameters, expectedReturnType);

            // Assert
            actual.Should().NotBeNull();
            actual.ExpressionName.Should().Be(expectedExpressionName);
            actual.Implementation.Should().Be(expectedImplementation);
            actual.Parameters.Should().BeSameAs(expectedParameters);
            actual.ReturnType.Should().Be(expectedReturnType);
        }
    }
}