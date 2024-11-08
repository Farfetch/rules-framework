namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class RuleOperationResultTests
    {
        [Fact]
        public void Failure_GivenCollectionOfErrors_ReturnsRuleOperationResultWithErrorsAndNoSuccess()
        {
            // Arrange
            IEnumerable<string> errors = new[] { "Error1", "Error2" };

            // Act
            var ruleOperationResult = OperationResult.Failure(errors);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeFalse();
            ruleOperationResult.Errors.Should().BeSameAs(errors);
        }

        [Fact]
        public void Failure_GivenNullCollectionOfErrors_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<string> errors = null;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => OperationResult.Failure(errors));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(errors));
        }

        [Fact]
        public void Success_NoConditionGiven_ReturnsRuleOperationResultWithoutErrorsAndSuccess()
        {
            // Act
            var ruleOperationResult = OperationResult.Success();

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();
            ruleOperationResult.Errors.Should().NotBeNull()
                .And.BeEmpty();
        }
    }
}