namespace Rules.Framework.WebUI.Tests.Extensions
{
    using System;
    using FluentAssertions;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Extensions;
    using Xunit;

    public class RuleDtoExtensionsTests
    {
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public RuleDtoExtensionsTests()
        {
            this.ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
        }

        [Fact]
        public void RuleDtoExtensions_ToRuleDto_Success()
        {
            // Arrange
            var genericRule = Rule.Create("Rule #1")
                .InRuleset("Ruleset #1")
                .SetContent(new object())
                .Since(new DateTime(2024, 6, 1))
                .Build().Rule;

            // Act
            var ruleDto = genericRule.ToRuleDto(this.ruleStatusDtoAnalyzer);

            // Assert
            ruleDto.Should().NotBeNull();
            ruleDto.Should().BeOfType<RuleDto>();
        }
    }
}