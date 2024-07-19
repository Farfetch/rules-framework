namespace Rules.Framework.Tests.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Generic;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesEngineTests
    {
        private readonly Mock<IRulesEngine> mockRulesEngine;

        public RulesEngineTests()
        {
            this.mockRulesEngine = new Mock<IRulesEngine>();
        }

        [Fact]
        public async Task GenericRulesEngine_GetContentTypes_Success()
        {
            // Arrange
            var expectedGenericContentTypes = new List<ContentType>
            {
                ContentType.Type1,
                ContentType.Type2,
            };

            var contentTypes = new[] { "Type1", "Type2" };
            this.mockRulesEngine.Setup(x => x.GetContentTypesAsync())
                .ReturnsAsync(contentTypes);

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericContentTypes = await genericRulesEngine.GetContentTypesAsync();

            // Assert
            genericContentTypes.Should().BeEquivalentTo(expectedGenericContentTypes);
        }

        [Fact]
        public async Task GenericRulesEngine_GetContentTypes_WithEmptyContentType_Success()
        {
            // Arrange
            var mockRulesEngineEmptyContentType = new Mock<IRulesEngine>();

            var genericRulesEngine = new RulesEngine<EmptyContentType, ConditionType>(mockRulesEngineEmptyContentType.Object);

            // Act
            var genericContentTypes = await genericRulesEngine.GetContentTypesAsync();

            // Assert
            genericContentTypes.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericRulesEngine_SearchAsync_Success()
        {
            // Arrange
            var expectedRule = Rule.New<ContentType, ConditionType>()
                .WithName("Test rule")
                .WithDatesInterval(new DateTime(2018, 01, 01), new DateTime(2019, 01, 01))
                .WithContent(ContentType.Type1, new object())
                .WithCondition(ConditionType.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            expectedRule.Priority = 3;
            var expectedGenericRules = new List<Rule>
            {
                expectedRule,
            };

            var dateBegin = new DateTime(2022, 01, 01);
            var dateEnd = new DateTime(2022, 12, 01);
            var genericContentType = ContentType.Type1;

            var genericSearchArgs = new SearchArgs<ContentType, ConditionType>(genericContentType, dateBegin, dateEnd);

            var testRule = Rule.New<ContentType, ConditionType>()
                .WithName("Test rule")
                .WithDatesInterval(new DateTime(2018, 01, 01), new DateTime(2019, 01, 01))
                .WithContent(ContentType.Type1, new object())
                .WithCondition(ConditionType.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            testRule.Priority = 3;
            var testRules = new List<Rule>
            {
                testRule
            };

            this.mockRulesEngine
                .Setup(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                .ReturnsAsync(testRules);

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericRules = await genericRulesEngine.SearchAsync(genericSearchArgs);

            // Assert
            var actualRule = genericRules.First();
            actualRule.Should().BeEquivalentTo(expectedRule);
            this.mockRulesEngine
                .Verify(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
        }
    }
}