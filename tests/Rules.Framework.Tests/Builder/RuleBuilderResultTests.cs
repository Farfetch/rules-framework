namespace Rules.Framework.Tests.Builder
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleBuilderResultTests
    {
        [Fact]
        public void Failure_GivenAllValidParameters_ReturnsRuleBuilderResultWithFailure()
        {
            // Arrange
            IEnumerable<string> expectedErrors = new[] { "Error1", "Error2" };

            // Act
            RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult = RuleBuilderResult.Failure<ContentType, ConditionType>(expectedErrors);

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
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => RuleBuilderResult.Failure<ContentType, ConditionType>(expectedErrors));

            // Arrange
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("errors");
        }

        [Fact]
        public void Success_GivenAllValidParameters_ReturnsRuleBuilderResultWithSuccessAndNewRule()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = new Rule<ContentType, ConditionType>();

            // Act
            RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult = RuleBuilderResult.Success<ContentType, ConditionType>(rule);

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
            Rule<ContentType, ConditionType> rule = null;

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => RuleBuilderResult.Success<ContentType, ConditionType>(rule));

            // Arrange
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("rule");
        }
    }
}