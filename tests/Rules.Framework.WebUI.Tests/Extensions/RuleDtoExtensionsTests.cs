namespace Rules.Framework.WebUI.Tests.Extensions
{
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
            var genericRule = new GenericRule();
            var contentType = "contentType";
            // Act
            var ruleDto = genericRule.ToRuleDto(contentType, this.ruleStatusDtoAnalyzer);

            // Assert
            ruleDto.Should().NotBeNull();
            ruleDto.Should().BeOfType<RuleDto>();
        }
    }
}
