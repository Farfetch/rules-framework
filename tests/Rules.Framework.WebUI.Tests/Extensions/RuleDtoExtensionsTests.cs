namespace Rules.Framework.WebUI.Tests.Extensions
{
    using System;
    using FluentAssertions;
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
            var contentType = new GenericContentType
            {
                Identifier = "contentType",
            };
            var genericRule = GenericRule.Create("Test", contentType, new object(), true, DateTime.Parse("2023-01-01"), null, 1, null);

            // Act
            var ruleDto = genericRule.ToRuleDto(this.ruleStatusDtoAnalyzer);

            // Assert
            ruleDto.Should().NotBeNull();
            ruleDto.Should().BeOfType<RuleDto>();
        }
    }
}