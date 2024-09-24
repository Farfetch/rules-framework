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
            var genericRule = Rule.New()
                .WithName("Rule #1")
                .WithDateBegin(new DateTime(2024, 6, 1))
                .WithContent("Type #1", new object())
                .Build().Rule;
            var contentType = "contentType";
            // Act
            var ruleDto = genericRule.ToRuleDto(contentType, this.ruleStatusDtoAnalyzer);

            // Assert
            ruleDto.Should().NotBeNull();
            ruleDto.Should().BeOfType<RuleDto>();
        }
    }
}