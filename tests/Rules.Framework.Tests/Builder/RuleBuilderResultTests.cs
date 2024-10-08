namespace Rules.Framework.Tests.Builder
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Xunit;

    public class RuleBuilderResultTests
    {
        [Fact]
        public void Failure_GivenAllValidParameters_ReturnsRuleBuilderResultWithFailure()
        {
            // Arrange
            IEnumerable<string> expectedErrors = new[] { "Error1", "Error2" };

            // Act
            var ruleBuilderResult = RuleBuilderResult.Failure(expectedErrors);

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeFalse();
            ruleBuilderResult.Errors.Should().NotBeNull().And.BeEquivalentTo(expectedErrors);
            ruleBuilderResult.Rule.Should().BeNull();
        }

        [Fact]
        public void Failure_GivenNullErrorsCollection_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<string> expectedErrors = null;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => RuleBuilderResult.Failure(expectedErrors));

            // Arrange
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("errors");
        }

        [Fact]
        public void Success_GivenAllValidParameters_ReturnsRuleBuilderResultWithSuccessAndNewRule()
        {
            // Arrange
            var rule = new Rule();

            // Act
            var ruleBuilderResult = RuleBuilderResult.Success(rule);

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();
            ruleBuilderResult.Errors.Should().NotBeNull().And.BeEmpty();
            ruleBuilderResult.Rule.Should().NotBeNull().And.Be(rule);
        }

        [Fact]
        public void Success_GivenNullErrorsCollection_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => RuleBuilderResult.Success(rule));

            // Arrange
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("rule");
        }
    }
}