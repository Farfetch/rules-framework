namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionParametersConfigurationTests
    {
        [Fact]
        public void CreateParameter_GivenNameAndTypeAndParameterAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var expectedName = "testParameter";
            var expectedType = typeof(string);

            var expressionParametersConfiguration = new ExpressionParametersConfiguration();
            expressionParametersConfiguration.CreateParameter(expectedName, expectedType);

            // Act
            var action = FluentActions.Invoking(() => expressionParametersConfiguration.CreateParameter(expectedName, expectedType));

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain(expectedName);
        }

        [Fact]
        public void CreateParameter_GivenNameAndTypeAndParameterDoesNotExist_AddsNewParameterAndReturns()
        {
            // Arrange
            var expectedName = "testParameter";
            var expectedType = typeof(string);

            var expressionParametersConfiguration = new ExpressionParametersConfiguration();

            // Act
            var actual = expressionParametersConfiguration.CreateParameter(expectedName, expectedType);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(expectedName);
            actual.Type.Should().Be(expectedType);

            expressionParametersConfiguration.Parameters.Should().HaveCount(1)
                .And.ContainKey(expectedName)
                .And.ContainValue(actual);
        }

        [Fact]
        public void CreateParameter_GivenNameAndTypeAsGenericAndParameterDoesNotExist_AddsNewParameterAndReturns()
        {
            // Arrange
            var expectedName = "testParameter";
            var expectedType = typeof(string);

            var expressionParametersConfiguration = new ExpressionParametersConfiguration();

            // Act
            var actual = expressionParametersConfiguration.CreateParameter<string>(expectedName);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(expectedName);
            actual.Type.Should().Be(expectedType);

            expressionParametersConfiguration.Parameters.Should().HaveCount(1)
                .And.ContainKey(expectedName)
                .And.ContainValue(actual);
        }
    }
}