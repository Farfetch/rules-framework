namespace Rules.Framework.WebUI.Tests.Extensions
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Generics;
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
            var genericRule = RuleBuilder.NewRule<string, string>()
                .WithName("test rule")
                .WithContent("contentType", new object())
                .WithDateBegin(DateTime.Parse("2024-01-01Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal))
                .Build().Rule;
            // Act
            var ruleDto = genericRule.ToRuleDto(this.ruleStatusDtoAnalyzer);

            // Assert
            ruleDto.Should().NotBeNull();
            ruleDto.Should().BeOfType<RuleDto>();
        }
    }
}