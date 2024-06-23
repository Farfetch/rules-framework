namespace Rules.Framework.Rql.IntegrationTests.Scenarios.Scenario8
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8;
    using Rules.Framework.Rql.Runtime.Types;
    using Xunit;

    public class TexasHoldEmPokerSingleCombinationsTests : IClassFixture<RulesEngineWithScenario8RulesFixture>
    {
        private readonly RulesEngineWithScenario8RulesFixture rulesEngineFixture;

        public TexasHoldEmPokerSingleCombinationsTests(
            RulesEngineWithScenario8RulesFixture rulesEngineFixture)
        {
            this.rulesEngineFixture = rulesEngineFixture;
        }

        [Theory]
        [MemberData(nameof(Scenario8TestCasesLoaderFixture.MatchAllTestCases), MemberType = typeof(Scenario8TestCasesLoaderFixture))]
        public async Task PokerCombinations_GivenMatchAllRqlStatement_EvaluatesAndReturnsResult(string rql, bool expectsRules, string[] ruleNames)
        {
            // Arrange
            var rqlEngine = this.rulesEngineFixture.RulesEngine.GetRqlEngine();

            // Act
            var results = await rqlEngine.ExecuteAsync(rql);

            // Assert
            results.Should().NotBeNull()
                .And.HaveCount(1);

            var result = results.First();
            result.Should().NotBeNull();

            if (expectsRules)
            {
                result.Should().BeOfType<RulesSetResult<ContentTypes, ConditionTypes>>();
                var rulesSetResult = (RulesSetResult<ContentTypes, ConditionTypes>)result;
                rulesSetResult.NumberOfRules.Should().Be(ruleNames.Length);
                rulesSetResult.Lines.Should().HaveCount(ruleNames.Length);

                for (int i = 0; i < ruleNames.Length; i++)
                {
                    var rule = rulesSetResult.Lines[i].Rule.Value;
                    rule.Name.Should().Be(ruleNames[i]);
                }
            }
            else
            {
                result.Should().BeOfType<ValueResult>();
                var valueResult = (ValueResult)result;
                valueResult.Value.Should().NotBeNull()
                    .And.BeOfType<RqlArray>()
                    .And.Subject.As<RqlArray>()
                    .Size.Value.Should().Be(0);
            }
        }

        [Theory]
        [MemberData(nameof(Scenario8TestCasesLoaderFixture.MatchOneTestCases), MemberType = typeof(Scenario8TestCasesLoaderFixture))]
        public async Task PokerCombinations_GivenMatchOneRqlStatement_EvaluatesAndReturnsResult(string rql, bool expectsRule, string ruleName)
        {
            // Arrange
            var rqlEngine = this.rulesEngineFixture.RulesEngine.GetRqlEngine();

            // Act
            var results = await rqlEngine.ExecuteAsync(rql);

            // Assert
            results.Should().NotBeNull()
                .And.HaveCount(1);

            var result = results.First();
            result.Should().NotBeNull();

            if (expectsRule)
            {
                result.Should().BeOfType<RulesSetResult<ContentTypes, ConditionTypes>>();
                var rulesSetResult = (RulesSetResult<ContentTypes, ConditionTypes>)result;
                rulesSetResult.NumberOfRules.Should().Be(1);
                rulesSetResult.Lines.Should().HaveCount(1);

                var rule = rulesSetResult.Lines[0].Rule.Value;
                rule.Name.Should().Be(ruleName);
            }
            else
            {
                result.Should().BeOfType<ValueResult>();
                var valueResult = (ValueResult)result;
                valueResult.Value.Should().NotBeNull()
                    .And.BeOfType<RqlArray>()
                    .And.Subject.As<RqlArray>()
                    .Size.Value.Should().Be(0);
            }
        }

        [Theory]
        [MemberData(nameof(Scenario8TestCasesLoaderFixture.SearchTestCases), MemberType = typeof(Scenario8TestCasesLoaderFixture))]
        public async Task PokerCombinations_GivenSearchRqlStatement_EvaluatesAndReturnsResult(string rql, bool expectsRules, string[] ruleNames)
        {
            // Arrange
            var rqlEngine = this.rulesEngineFixture.RulesEngine.GetRqlEngine();

            // Act
            var results = await rqlEngine.ExecuteAsync(rql);

            // Assert
            results.Should().NotBeNull()
                .And.HaveCount(1);

            var result = results.First();
            result.Should().NotBeNull();

            if (expectsRules)
            {
                result.Should().BeOfType<RulesSetResult<ContentTypes, ConditionTypes>>();
                var rulesSetResult = (RulesSetResult<ContentTypes, ConditionTypes>)result;
                rulesSetResult.NumberOfRules.Should().Be(ruleNames.Length);
                rulesSetResult.Lines.Should().HaveCount(ruleNames.Length);

                for (int i = 0; i < ruleNames.Length; i++)
                {
                    var rule = rulesSetResult.Lines[i].Rule.Value;
                    rule.Name.Should().Be(ruleNames[i]);
                }
            }
            else
            {
                result.Should().BeOfType<ValueResult>();
                var valueResult = (ValueResult)result;
                valueResult.Value.Should().NotBeNull()
                    .And.BeOfType<RqlArray>()
                    .And.Subject.As<RqlArray>()
                    .Size.Value.Should().Be(0);
            }
        }
    }
}