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
        private readonly IRulesEngine rulesEngineMock;

        public RulesEngineTests()
        {
            this.rulesEngineMock = Mock.Of<IRulesEngine>();
        }

        [Fact]
        public async Task GetContentTypes_NoConditionsGiven_ReturnsContentTypes()
        {
            // Arrange
            var expectedGenericContentTypes = new List<ContentType>
            {
                ContentType.Type1,
                ContentType.Type2,
            };

            var contentTypes = new[] { "Type1", "Type2" };
            Mock.Get(this.rulesEngineMock).Setup(x => x.GetContentTypesAsync())
                .ReturnsAsync(contentTypes);

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(this.rulesEngineMock);

            // Act
            var genericContentTypes = await genericRulesEngine.GetContentTypesAsync();

            // Assert
            genericContentTypes.Should().BeEquivalentTo(expectedGenericContentTypes);
        }

        [Fact]
        public async Task GetContentTypes_WithEmptyContentType_Success()
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
        public async Task GetUniqueConditionTypes_GivenContentTypeAndDatesInterval_ReturnsConditionTypes()
        {
            // Arrange
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetUniqueConditionTypesAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { nameof(ConditionType.NumberOfSales), nameof(ConditionType.IsVip), });

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(rulesEngineMock);

            // Act
            var genericContentTypes = await genericRulesEngine.GetUniqueConditionTypesAsync(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue);

            // Assert
            genericContentTypes.Should().NotBeNullOrEmpty()
                .And.Contain(ConditionType.NumberOfSales)
                .And.Contain(ConditionType.IsVip);
        }

        [Fact]
        public void Options_PropertyGet_ReturnsRulesEngineOptions()
        {
            // Arrange
            var options = RulesEngineOptions.NewWithDefaults();
            Mock.Get(this.rulesEngineMock)
                .SetupGet(x => x.Options)
                .Returns(options);

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(this.rulesEngineMock);

            // Act
            var actual = genericRulesEngine.Options;

            // Assert
            actual.Should().BeSameAs(options);
        }

        [Fact]
        public async Task SearchAsync_GivenContentTypeAndDatesIntervalAndNoConditions_ReturnsRules()
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

            Mock.Get(this.rulesEngineMock)
                .Setup(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                .ReturnsAsync(testRules);

            var genericRulesEngine = new RulesEngine<ContentType, ConditionType>(this.rulesEngineMock);

            // Act
            var genericRules = await genericRulesEngine.SearchAsync(genericSearchArgs);

            // Assert
            var actualRule = genericRules.First();
            actualRule.Should().BeEquivalentTo(expectedRule);
            Mock.Get(this.rulesEngineMock)
                .Verify(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(nameof(RulesEngine<ContentType, ConditionType>.ActivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<ContentType, ConditionType>.AddRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<ContentType, ConditionType>.DeactivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<ContentType, ConditionType>.SearchAsync), "searchArgs", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<ContentType, ConditionType>.UpdateRuleAsync), "rule", typeof(ArgumentNullException))]
        public async Task VerifyParameters_GivenNullParameter_ThrowsArgumentNullException(string methodName, string parameterName, Type exceptionType)
        {
            // Arrange
            var sut = new RulesEngine<ContentType, ConditionType>(this.rulesEngineMock);

            // Act
            var actual = await Assert.ThrowsAsync(exceptionType, async () =>
            {
                switch (methodName)
                {
                    case nameof(RulesEngine<ContentType, ConditionType>.ActivateRuleAsync):
                        _ = await sut.ActivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<ContentType, ConditionType>.AddRuleAsync):
                        _ = await sut.AddRuleAsync(null, RuleAddPriorityOption.AtTop);
                        break;

                    case nameof(RulesEngine<ContentType, ConditionType>.DeactivateRuleAsync):
                        _ = await sut.DeactivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<ContentType, ConditionType>.SearchAsync):
                        _ = await sut.SearchAsync(null);
                        break;

                    case nameof(RulesEngine<ContentType, ConditionType>.UpdateRuleAsync):
                        _ = await sut.UpdateRuleAsync(null);
                        break;

                    default:
                        Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                        break;
                }
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType(exceptionType);
            if (actual is ArgumentException argumentException)
            {
                argumentException.Message.Should().Contain(parameterName);
                argumentException.ParamName.Should().Be(parameterName);
            }
        }
    }
}