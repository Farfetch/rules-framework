namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class RuleOperationResultTests
    {
        [Fact]
        public void Error_GivenCollectionOfErrors_ReturnsRuleOperationResultWithErrorsAndNoSuccess()
        {
            // Arrange
            IEnumerable<string> errors = new[] { "Error1", "Error2" };

            // Act
            RuleOperationResult ruleOperationResult = RuleOperationResult.Error(errors);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeFalse();
            ruleOperationResult.Errors.Should().BeSameAs(errors);
        }

        [Fact]
        public void Error_GivenNullCollectionOfErrors_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<string> errors = null;

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => RuleOperationResult.Error(errors));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(errors));
        }

        [Fact]
        public void Success_NoConditionGiven_ReturnsRuleOperationResultWithoutErrorsAndSuccess()
        {
            // Act
            RuleOperationResult ruleOperationResult = RuleOperationResult.Success();

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();
            ruleOperationResult.Errors.Should().NotBeNull()
                .And.BeEmpty();
        }
    }
}