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
        public async Task GetRulesetsAsync_NoConditionsGiven_ReturnsRulesets()
        {
            // Arrange
            var ruleset1 = new Ruleset("Type1", DateTime.UtcNow);
            var ruleset2 = new Ruleset("Type2", DateTime.UtcNow);
            var rulesets = new[] { ruleset1, ruleset2, };
            var expectedGenericRulesets = new[]
            {
                new Ruleset<RulesetNames>(ruleset1),
                new Ruleset<RulesetNames>(ruleset2),
            };
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(rulesets);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericRulesets = await genericRulesEngine.GetRulesetsAsync();

            // Assert
            genericRulesets.Should().BeEquivalentTo(expectedGenericRulesets);
        }

        [Fact]
        public async Task GetRulesetsAsync_WithEmptyRulesetsNames_ReturnsEmptyRulesetsCollection()
        {
            // Arrange
            var mockRulesEngineEmptyContentType = new Mock<IRulesEngine>();

            var genericRulesEngine = new RulesEngine<EmptyRulesetNames, ConditionNames>(mockRulesEngineEmptyContentType.Object);

            // Act
            var genericRulesets = await genericRulesEngine.GetRulesetsAsync();

            // Assert
            genericRulesets.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUniqueConditions_GivenRulesetAndDatesInterval_ReturnsConditions()
        {
            // Arrange
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetUniqueConditionsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { nameof(ConditionNames.NumberOfSales), nameof(ConditionNames.IsVip), });

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(rulesEngineMock);

            // Act
            var genericConditions = await genericRulesEngine.GetUniqueConditionsAsync(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue);

            // Assert
            genericConditions.Should().NotBeNullOrEmpty()
                .And.Contain(ConditionNames.NumberOfSales)
                .And.Contain(ConditionNames.IsVip);
        }

        [Fact]
        public void Options_PropertyGet_ReturnsRulesEngineOptions()
        {
            // Arrange
            var options = RulesEngineOptions.NewWithDefaults();
            Mock.Get(this.rulesEngineMock)
                .SetupGet(x => x.Options)
                .Returns(options);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var actual = genericRulesEngine.Options;

            // Assert
            actual.Should().BeSameAs(options);
        }

        [Fact]
        public async Task SearchAsync_GivenContentTypeAndDatesIntervalAndNoConditions_ReturnsRules()
        {
            // Arrange
            var expectedRule = Rule.Create<RulesetNames, ConditionNames>("Test rule")
                .OnRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(new DateTime(2018, 01, 01))
                .Until(new DateTime(2019, 01, 01))
                .ApplyWhen(ConditionNames.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            expectedRule.Priority = 3;

            var dateBegin = new DateTime(2022, 01, 01);
            var dateEnd = new DateTime(2022, 12, 01);
            var genericContentType = RulesetNames.Type1;

            var genericSearchArgs = new SearchArgs<RulesetNames, ConditionNames>(genericContentType, dateBegin, dateEnd);

            var testRule = Rule.Create<RulesetNames, ConditionNames>("Test rule")
                .OnRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(new DateTime(2018, 01, 01))
                .Until(new DateTime(2019, 01, 01))
                .ApplyWhen(ConditionNames.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            testRule.Priority = 3;
            var testRules = new List<Rule>
            {
                testRule
            };

            Mock.Get(this.rulesEngineMock)
                .Setup(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                .ReturnsAsync(testRules);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericRules = await genericRulesEngine.SearchAsync(genericSearchArgs);

            // Assert
            var actualRule = genericRules.First();
            actualRule.Should().BeEquivalentTo(expectedRule);
            Mock.Get(this.rulesEngineMock)
                .Verify(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.ActivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.AddRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.DeactivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.SearchAsync), "searchArgs", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.UpdateRuleAsync), "rule", typeof(ArgumentNullException))]
        public async Task VerifyParameters_GivenNullParameter_ThrowsArgumentNullException(string methodName, string parameterName, Type exceptionType)
        {
            // Arrange
            var sut = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var actual = await Assert.ThrowsAsync(exceptionType, async () =>
            {
                switch (methodName)
                {
                    case nameof(RulesEngine<RulesetNames, ConditionNames>.ActivateRuleAsync):
                        _ = await sut.ActivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.AddRuleAsync):
                        _ = await sut.AddRuleAsync(null, RuleAddPriorityOption.AtTop);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.DeactivateRuleAsync):
                        _ = await sut.DeactivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.SearchAsync):
                        _ = await sut.SearchAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.UpdateRuleAsync):
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